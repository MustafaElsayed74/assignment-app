using System.Text.Json.Serialization;

namespace BlockedCountriesApi.Models;

/// <summary>
/// Represents the geolocation result returned from the ipapi.co API.
/// </summary>
public class IpLookupResult
{
    /// <summary>
    /// The IP address that was looked up.
    /// </summary>
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// ISO 3166-1 alpha-2 country code.
    /// </summary>
    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the country.
    /// </summary>
    [JsonPropertyName("country_name")]
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// City name.
    /// </summary>
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Region/state name.
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Internet Service Provider / Organization.
    /// </summary>
    [JsonPropertyName("org")]
    public string Isp { get; set; } = string.Empty;

    /// <summary>
    /// Timezone (e.g., "America/New_York").
    /// </summary>
    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = string.Empty;
}
