using System.Globalization;
using BlockedCountriesApi.Helpers;
using BlockedCountriesApi.Models;
using BlockedCountriesApi.Models.Responses;
using BlockedCountriesApi.Repositories;

namespace BlockedCountriesApi.Services;

public class CountryService : ICountryService
{
    private readonly IBlockedCountryRepository _blockedCountryRepository;
    private readonly ITemporalBlockRepository _temporalBlockRepository;

    public CountryService(IBlockedCountryRepository blockedCountryRepository, ITemporalBlockRepository temporalBlockRepository)
    {
        _blockedCountryRepository = blockedCountryRepository;
        _temporalBlockRepository = temporalBlockRepository;
    }

    public bool BlockCountry(string countryCode)
    {
        countryCode = countryCode.ToUpperInvariant();
        if (!CountryCodeValidator.IsValid(countryCode))
        {
            throw new ArgumentException("Invalid ISO 3166-1 alpha-2 country code.");
        }

        if (_blockedCountryRepository.Exists(countryCode))
        {
            return false; // Already blocked
        }

        var country = new BlockedCountry
        {
            CountryCode = countryCode,
            CountryName = GetCountryName(countryCode)
        };

        // If it was temporarily blocked, removing it from there as it is now permanently blocked
        _temporalBlockRepository.Remove(countryCode);

        return _blockedCountryRepository.Add(country);
    }

    public bool UnblockCountry(string countryCode)
    {
        countryCode = countryCode.ToUpperInvariant();
        bool removedPermanent = _blockedCountryRepository.Remove(countryCode);
        bool removedTemporal = _temporalBlockRepository.Remove(countryCode);
        
        return removedPermanent || removedTemporal;
    }

    public PagedResponse<object> GetBlockedCountries(int page, int pageSize, string? search)
    {
        // Combine permanent and active temporal blocks
        var permanentBlocks = _blockedCountryRepository.GetAll().Select(c => new
        {
            c.CountryCode,
            c.CountryName,
            c.BlockedAt,
            IsTemporal = false,
            ExpiresAt = (DateTime?)null
        });

        var temporalBlocks = _temporalBlockRepository.GetAll().Select(t => new
        {
            t.CountryCode,
            t.CountryName,
            t.BlockedAt,
            IsTemporal = true,
            ExpiresAt = (DateTime?)t.ExpiresAt
        });

        var allBlocks = permanentBlocks.Concat(temporalBlocks).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            allBlocks = allBlocks.Where(c => 
                c.CountryCode.ToLowerInvariant().Contains(searchLower) || 
                c.CountryName.ToLowerInvariant().Contains(searchLower));
        }

        var totalCount = allBlocks.Count();
        var items = allBlocks
            .OrderByDescending(c => c.BlockedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResponse<object>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public bool IsCountryBlocked(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return false;

        countryCode = countryCode.ToUpperInvariant();
        return _blockedCountryRepository.Exists(countryCode) || _temporalBlockRepository.Exists(countryCode);
    }

    public bool TemporalBlockCountry(string countryCode, int durationMinutes)
    {
        countryCode = countryCode.ToUpperInvariant();
        if (!CountryCodeValidator.IsValid(countryCode))
        {
            throw new ArgumentException("Invalid ISO 3166-1 alpha-2 country code.");
        }

        if (_blockedCountryRepository.Exists(countryCode) || _temporalBlockRepository.Exists(countryCode))
        {
            return false; // Already blocked
        }

        var block = new TemporalBlock
        {
            CountryCode = countryCode,
            CountryName = GetCountryName(countryCode),
            BlockedAt = DateTime.UtcNow,
            DurationMinutes = durationMinutes,
            ExpiresAt = DateTime.UtcNow.AddMinutes(durationMinutes)
        };

        return _temporalBlockRepository.Add(block);
    }

    // Helper to get country name from code (fallback to code if not found)
    public string GetCountryName(string countryCode)
    {
        try
        {
            var regionInfo = new RegionInfo(countryCode);
            return regionInfo.EnglishName;
        }
        catch
        {
            return countryCode; // Fallback
        }
    }
}
