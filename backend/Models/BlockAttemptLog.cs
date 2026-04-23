namespace BlockedCountriesApi.Models;

/// <summary>
/// Represents a log entry for an IP block-check attempt.
/// </summary>
public class BlockAttemptLog
{
    /// <summary>
    /// The IP address that was checked.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp of when the check was performed.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The country code resolved from the IP address.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether the country associated with this IP is currently blocked.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// The User-Agent header from the HTTP request.
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;
}
