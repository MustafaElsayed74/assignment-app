using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Repositories;

public interface ITemporalBlockRepository
{
    bool Add(TemporalBlock block);
    bool Remove(string countryCode);
    TemporalBlock? Get(string countryCode);
    IEnumerable<TemporalBlock> GetAll();
    bool Exists(string countryCode);
    int RemoveExpired();
}
