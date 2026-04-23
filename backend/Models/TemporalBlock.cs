namespace BlockedCountriesApi.Models;

/// <summary>
/// Represents a temporarily blocked country that automatically unblocks after expiration.
/// </summary>
public class TemporalBlock
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the country.
    /// </summary>
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the temporal block was created.
    /// </summary>
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp when this temporal block expires and the country is automatically unblocked.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Duration of the block in minutes (1–1440).
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Returns true if the temporal block has expired.
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
