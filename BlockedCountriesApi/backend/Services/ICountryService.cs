using BlockedCountriesApi.Models;
using BlockedCountriesApi.Models.Responses;

namespace BlockedCountriesApi.Services;

public interface ICountryService
{
    bool BlockCountry(string countryCode);
    bool UnblockCountry(string countryCode);
    PagedResponse<object> GetBlockedCountries(int page, int pageSize, string? search);
    bool IsCountryBlocked(string countryCode);
    bool TemporalBlockCountry(string countryCode, int durationMinutes);
    string GetCountryName(string countryCode); // Helper for tests/internal use
}
