using BlockedCountriesApi.Models.Responses;
using BlockedCountriesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly IBlockAttemptService _blockAttemptService;

    public LogsController(IBlockAttemptService blockAttemptService)
    {
        _blockAttemptService = blockAttemptService;
    }

    /// <summary>
    /// Log Failed Blocked Attempts (paginated).
    /// </summary>
    [HttpGet("blocked-attempts")]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    public IActionResult GetBlockedAttempts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var response = _blockAttemptService.GetBlockedAttempts(page, pageSize);
        return Ok(response);
    }
}
