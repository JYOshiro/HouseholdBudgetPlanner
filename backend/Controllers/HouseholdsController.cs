using HouseholdBudgetApi.DTOs.Household;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Household controller for managing household data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HouseholdsController : ControllerBase
{
    private readonly IHouseholdService _householdService;
    private readonly ILogger<HouseholdsController> _logger;

    public HouseholdsController(IHouseholdService householdService, ILogger<HouseholdsController> logger)
    {
        _householdService = householdService;
        _logger = logger;
    }

    /// <summary>
    /// Gets household details.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<HouseholdDto>> GetHousehold()
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetHousehold called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var household = await _householdService.GetHouseholdAsync(householdId);
        if (household == null)
        {
            return NotFound(new { message = "Household not found" });
        }

        return Ok(household);
    }

    /// <summary>
    /// Gets household members.
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<IEnumerable<HouseholdMemberDto>>> GetMembers()
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetMembers called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var members = await _householdService.GetHouseholdMembersAsync(householdId);
        return Ok(members);
    }
}
