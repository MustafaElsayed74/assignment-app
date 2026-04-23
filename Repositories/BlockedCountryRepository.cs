using System.Collections.Concurrent;
using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Repositories;

public class BlockedCountryRepository : IBlockedCountryRepository
{
    private readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = new(StringComparer.OrdinalIgnoreCase);

    public bool Add(BlockedCountry country)
    {
        return _blockedCountries.TryAdd(country.CountryCode, country);
    }

    public bool Remove(string countryCode)
    {
        return _blockedCountries.TryRemove(countryCode, out _);
    }

    public BlockedCountry? Get(string countryCode)
    {
        _blockedCountries.TryGetValue(countryCode, out var country);
        return country;
    }

    public IEnumerable<BlockedCountry> GetAll()
    {
        return _blockedCountries.Values;
    }

    public bool Exists(string countryCode)
    {
        return _blockedCountries.ContainsKey(countryCode);
    }
}
