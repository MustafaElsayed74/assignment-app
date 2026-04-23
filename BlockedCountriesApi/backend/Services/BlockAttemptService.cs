using BlockedCountriesApi.Models;
using BlockedCountriesApi.Models.Responses;
using BlockedCountriesApi.Repositories;

namespace BlockedCountriesApi.Services;

public class BlockAttemptService : IBlockAttemptService
{
    private readonly IIpGeolocationService _ipGeolocationService;
    private readonly ICountryService _countryService;
    private readonly IBlockAttemptLogRepository _logRepository;
    private readonly ILogger<BlockAttemptService> _logger;

    public BlockAttemptService(
        IIpGeolocationService ipGeolocationService,
        ICountryService countryService,
        IBlockAttemptLogRepository logRepository,
        ILogger<BlockAttemptService> logger)
    {
        _ipGeolocationService = ipGeolocationService;
        _countryService = countryService;
        _logRepository = logRepository;
        _logger = logger;
    }

    public async Task<IpBlockCheckResponse> CheckAndLogAttemptAsync(string ipAddress, string userAgent)
    {
        var response = new IpBlockCheckResponse { IpAddress = ipAddress };

        // Localhost/Loopback handling
        if (ipAddress == "127.0.0.1" || ipAddress == "::1")
        {
            response.CountryCode = "Local";
            response.CountryName = "Localhost";
            response.IsBlocked = false;
        }
        else
        {
            var lookupResult = await _ipGeolocationService.LookupIpAsync(ipAddress);
            
            if (lookupResult != null && !string.IsNullOrEmpty(lookupResult.CountryCode))
            {
                response.CountryCode = lookupResult.CountryCode;
                response.CountryName = lookupResult.CountryName;
                response.IsBlocked = _countryService.IsCountryBlocked(response.CountryCode);
            }
            else
            {
                _logger.LogWarning("Could not resolve country for IP {IpAddress}", ipAddress);
                // If we can't resolve, we don't block
                response.CountryCode = "Unknown";
                response.CountryName = "Unknown";
                response.IsBlocked = false;
            }
        }

        if (response.IsBlocked)
        {
            var log = new BlockAttemptLog
            {
                IpAddress = ipAddress,
                CountryCode = response.CountryCode,
                IsBlocked = true,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _logRepository.Add(log);
        }

        return response;
    }

    public PagedResponse<object> GetBlockedAttempts(int page, int pageSize)
    {
        var allLogs = _logRepository.GetAll();
        var totalCount = allLogs.Count();

        var items = allLogs
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new
            {
                l.IpAddress,
                l.Timestamp,
                l.CountryCode,
                l.IsBlocked,
                l.UserAgent
            })
            .ToList<object>();

        return new PagedResponse<object>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
