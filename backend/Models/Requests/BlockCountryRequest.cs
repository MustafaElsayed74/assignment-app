using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesApi.Models.Requests;

/// <summary>
/// Request body for adding a country to the blocked list.
/// </summary>
public class BlockCountryRequest
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "US", "GB", "EG").
    /// </summary>
    [Required(ErrorMessage = "Country code is required.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be exactly 2 characters.")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Country code must be 2 uppercase letters (e.g., 'US', 'GB').")]
    public string CountryCode { get; set; } = string.Empty;
}
