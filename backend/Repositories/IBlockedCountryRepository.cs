using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Repositories;

public interface IBlockedCountryRepository
{
    bool Add(BlockedCountry country);
    bool Remove(string countryCode);
    BlockedCountry? Get(string countryCode);
    IEnumerable<BlockedCountry> GetAll();
    bool Exists(string countryCode);
}
