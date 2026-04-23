namespace BlockedCountriesApi.Models.Responses;

/// <summary>
/// Standardized API error response envelope.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional details about the error (optional).
    /// </summary>
    public string? Details { get; set; }
}
