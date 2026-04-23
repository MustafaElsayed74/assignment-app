using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesApi.Models.Requests;

/// <summary>
/// Request body for temporarily blocking a country for a specified duration.
/// </summary>
public class TemporalBlockRequest
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "US", "GB", "EG").
    /// </summary>
    [Required(ErrorMessage = "Country code is required.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be exactly 2 characters.")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Country code must be 2 uppercase letters (e.g., 'US', 'GB').")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Block duration in minutes. Must be between 1 and 1440 (24 hours).
    /// </summary>
    [Required(ErrorMessage = "Duration in minutes is required.")]
    [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes (24 hours).")]
    public int DurationMinutes { get; set; }
}
