using BlockedCountriesApi.Helpers;
using BlockedCountriesApi.Models;
using BlockedCountriesApi.Models.Responses;
using BlockedCountriesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IpController : ControllerBase
{
    private readonly IIpGeolocationService _ipGeolocationService;
    private readonly IBlockAttemptService _blockAttemptService;

    public IpController(IIpGeolocationService ipGeolocationService, IBlockAttemptService blockAttemptService)
    {
        _ipGeolocationService = ipGeolocationService;
        _blockAttemptService = blockAttemptService;
    }

    /// <summary>
    /// Find Country via IP Lookup.
    /// If ipAddress is omitted, uses the caller's IP.
    /// </summary>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IpLookupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> LookupIp([FromQuery] string? ipAddress)
    {
        var ipToLookup = ipAddress;
        
        if (string.IsNullOrWhiteSpace(ipToLookup))
        {
            ipToLookup = IpAddressHelper.GetClientIpAddress(HttpContext);
        }

        if (!IpAddressHelper.IsValidIpAddress(ipToLookup))
        {
            return BadRequest(new ApiErrorResponse 
            { 
                StatusCode = 400, 
                Message = $"Invalid IP address format: {ipToLookup}" 
            });
        }

        // Handle localhost requests for local testing gracefully
        if (ipToLookup == "127.0.0.1" || ipToLookup == "::1")
        {
            return Ok(new IpLookupResult
            {
                Ip = ipToLookup,
                CountryCode = "Local",
                CountryName = "Localhost",
                City = "Local",
                Region = "Local",
                Isp = "Local"
            });
        }

        var result = await _ipGeolocationService.LookupIpAsync(ipToLookup);

        if (result == null || string.IsNullOrEmpty(result.CountryCode))
        {
            return StatusCode(502, new ApiErrorResponse 
            { 
                StatusCode = 502, 
                Message = "Failed to resolve geolocation for the provided IP address. Third-party API might be unavailable or rate-limited." 
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Verify If caller's IP is Blocked and log the attempt.
    /// </summary>
    [HttpGet("check-block")]
    [ProducesResponseType(typeof(IpBlockCheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckBlock()
    {
        var ipAddress = IpAddressHelper.GetClientIpAddress(HttpContext);
        var userAgent = Request.Headers["User-Agent"].ToString();

        var result = await _blockAttemptService.CheckAndLogAttemptAsync(ipAddress, userAgent);

        return Ok(result);
    }
}
