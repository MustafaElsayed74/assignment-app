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
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
            
            var apiResult = JsonSerializer.Deserialize<IpApiResult>(content, options);

            // Handle cases where ipapi.co returns an error object (e.g., rate limit) within a 200 OK
            if (apiResult != null && string.IsNullOrEmpty(apiResult.CountryCode) && content.Contains("error"))
            {
                _logger.LogWarning("IP API returned an error for IP {IpAddress}: {Content}", ipAddress, content);
                return null;
            }

            if (apiResult == null) return null;

            return new IpLookupResult
            {
                Ip = apiResult.Ip ?? "",
                CountryCode = apiResult.CountryCode ?? "",
                CountryName = apiResult.CountryName ?? "",
                City = apiResult.City ?? "",
                Region = apiResult.Region ?? "",
                Isp = apiResult.Org ?? "",
                Timezone = apiResult.Timezone ?? ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while looking up IP {IpAddress}", ipAddress);
            return null;
        }
    }

    private class IpApiResult
    {
        public string? Ip { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Org { get; set; }
        public string? Timezone { get; set; }
    }
}
