using HouseholdBudgetApi.DTOs.Dashboard;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Dashboard controller for providing summary data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Gets dashboard summary for the current or specified month.
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary([FromQuery] int? year = null, [FromQuery] int? month = null)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetSummary called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var targetDate = DateTime.UtcNow;
        var targetYear = year ?? targetDate.Year;
        var targetMonth = month ?? targetDate.Month;

        if (targetMonth is < 1 or > 12)
        {
            return BadRequest(new { message = "Month must be between 1 and 12." });
        }

        var summary = await _dashboardService.GetDashboardSummaryAsync(householdId, targetYear, targetMonth);
        return Ok(summary);
    }
}
