using HouseholdBudgetApi.DTOs.SavingsGoal;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Savings Goals controller for managing savings goals.
/// </summary>
[ApiController]
[Route("api/savings-goals")]
[Authorize]
public class SavingsGoalsController : ControllerBase
{
    private readonly ISavingsGoalService _savingsGoalService;
    private readonly ILogger<SavingsGoalsController> _logger;

    public SavingsGoalsController(ISavingsGoalService savingsGoalService, ILogger<SavingsGoalsController> logger)
    {
        _savingsGoalService = savingsGoalService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all savings goals for the user's household.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavingsGoalDto>>> GetGoals()
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetGoals called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var goals = await _savingsGoalService.GetGoalsAsync(householdId);
        return Ok(goals);
    }

    /// <summary>
    /// Gets a single savings goal by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SavingsGoalDto>> GetGoal(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetGoal called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var goal = await _savingsGoalService.GetGoalAsync(id, householdId);
        if (goal == null)
        {
            return NotFound(new { message = "Savings goal not found" });
        }

        return Ok(goal);
    }

    /// <summary>
    /// Creates a new savings goal.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SavingsGoalDto>> CreateGoal([FromBody] CreateSavingsGoalDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("CreateGoal called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var goal = await _savingsGoalService.CreateGoalAsync(householdId, request);
        return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
    }

    /// <summary>
    /// Updates a savings goal.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<SavingsGoalDto>> UpdateGoal(int id, [FromBody] UpdateSavingsGoalDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("UpdateGoal called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var goal = await _savingsGoalService.UpdateGoalAsync(id, householdId, request);
        return Ok(goal);
    }

    /// <summary>
    /// Deletes a savings goal.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoal(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("DeleteGoal called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        await _savingsGoalService.DeleteGoalAsync(id, householdId);
        return NoContent();
    }
}
