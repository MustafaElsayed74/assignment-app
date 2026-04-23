namespace BlockedCountriesApi.Models;

/// <summary>
/// Represents a permanently blocked country stored in the in-memory blocked list.
/// </summary>
public class BlockedCountry
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "US", "GB", "EG").
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the country (e.g., "United States", "United Kingdom").
    /// </summary>
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when this country was added to the blocked list.
    /// </summary>
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
}
