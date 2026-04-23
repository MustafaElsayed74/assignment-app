using BlockedCountriesApi.Models.Requests;
using BlockedCountriesApi.Models.Responses;
using BlockedCountriesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Add a Country to the blocked list.
    /// </summary>
    [HttpPost("block")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
    {
        try
        {
            var added = _countryService.BlockCountry(request.CountryCode);
            if (!added)
            {
                return Conflict(new ApiErrorResponse 
                { 
                    StatusCode = 409, 
                    Message = $"Country '{request.CountryCode}' is already blocked." 
                });
            }

            return Created("", new { Message = $"Country '{request.CountryCode}' blocked successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiErrorResponse { StatusCode = 400, Message = ex.Message });
        }
    }

    /// <summary>
    /// Remove a Country from the blocked list.
    /// </summary>
    [HttpDelete("block/{countryCode}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult UnblockCountry(string countryCode)
    {
        var removed = _countryService.UnblockCountry(countryCode);
        if (!removed)
        {
            return NotFound(new ApiErrorResponse 
            { 
                StatusCode = 404, 
                Message = $"Country '{countryCode}' is not currently blocked." 
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Get All Blocked Countries (paginated and searchable).
    /// </summary>
    [HttpGet("blocked")]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    public IActionResult GetBlockedCountries(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? search = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Cap max page size

        var response = _countryService.GetBlockedCountries(page, pageSize, search);
        return Ok(response);
    }

    /// <summary>
    /// Temporarily Block a Country.
    /// </summary>
    [HttpPost("temporal-block")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public IActionResult TemporalBlockCountry([FromBody] TemporalBlockRequest request)
    {
        try
        {
            var added = _countryService.TemporalBlockCountry(request.CountryCode, request.DurationMinutes);
            if (!added)
            {
                return Conflict(new ApiErrorResponse 
                { 
                    StatusCode = 409, 
                    Message = $"Country '{request.CountryCode}' is already blocked (permanently or temporarily)." 
                });
            }

            return Created("", new 
            { 
                Message = $"Country '{request.CountryCode}' blocked temporarily for {request.DurationMinutes} minutes." 
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiErrorResponse { StatusCode = 400, Message = ex.Message });
        }
    }
}
