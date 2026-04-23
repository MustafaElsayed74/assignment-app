namespace BlockedCountriesApi.Models.Responses;

/// <summary>
/// Response returned when checking if a caller's IP is blocked.
/// </summary>
public class IpBlockCheckResponse
{
    /// <summary>
    /// The caller's IP address.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// The country code resolved from the IP address.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// The country name resolved from the IP address.
    /// </summary>
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the country is currently blocked.
    /// </summary>
    public bool IsBlocked { get; set; }
}
