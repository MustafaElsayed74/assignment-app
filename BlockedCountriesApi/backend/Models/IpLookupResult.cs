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
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// ISO 3166-1 alpha-2 country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the country.
    /// </summary>
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// City name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Region/state name.
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Internet Service Provider / Organization.
    /// </summary>
    public string Isp { get; set; } = string.Empty;

    /// <summary>
    /// Timezone (e.g., "America/New_York").
    /// </summary>
    public string Timezone { get; set; } = string.Empty;
}
