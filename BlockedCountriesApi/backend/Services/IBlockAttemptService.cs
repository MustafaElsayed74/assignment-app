using BlockedCountriesApi.Models.Responses;

namespace BlockedCountriesApi.Services;

public interface IBlockAttemptService
{
    Task<IpBlockCheckResponse> CheckAndLogAttemptAsync(string ipAddress, string userAgent);
    PagedResponse<object> GetBlockedAttempts(int page, int pageSize);
}
