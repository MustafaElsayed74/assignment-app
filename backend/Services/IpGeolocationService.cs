using System.Text.Json;
using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Services;

public class IpGeolocationService : IIpGeolocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IpGeolocationService> _logger;
    private readonly IConfiguration _configuration;

    public IpGeolocationService(IHttpClientFactory httpClientFactory, ILogger<IpGeolocationService> logger, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IpLookupResult?> LookupIpAsync(string ipAddress)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("IpApi");
            var baseUrl = _configuration["IpApi:BaseUrl"] ?? "https://ipapi.co";
            
            // Construct the URL. The ipapi.co format is: https://ipapi.co/{ip}/json/
            var requestUrl = $"{baseUrl.TrimEnd('/')}/{ipAddress}/json/";

            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to lookup IP {IpAddress}. Status Code: {StatusCode}", ipAddress, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IpLookupResult>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Handle cases where ipapi.co returns an error object (e.g., rate limit) within a 200 OK
            if (result != null && string.IsNullOrEmpty(result.CountryCode) && content.Contains("error"))
            {
                _logger.LogWarning("IP API returned an error for IP {IpAddress}: {Content}", ipAddress, content);
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while looking up IP {IpAddress}", ipAddress);
            return null;
        }
    }
}
