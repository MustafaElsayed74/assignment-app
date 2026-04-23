using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Services;

public interface IIpGeolocationService
{
    Task<IpLookupResult?> LookupIpAsync(string ipAddress);
}
