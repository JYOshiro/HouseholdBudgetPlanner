using HouseholdBudgetApi.DTOs.GoalContribution;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Goal Contributions controller for managing contributions to savings goals.
/// </summary>
[ApiController]
[Route("api/goals/{goalId}/contributions")]
[Authorize]
public class GoalContributionsController : ControllerBase
{
    private readonly IGoalContributionService _goalContributionService;
    private readonly ILogger<GoalContributionsController> _logger;

    public GoalContributionsController(IGoalContributionService goalContributionService, ILogger<GoalContributionsController> logger)
    {
        _goalContributionService = goalContributionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all contributions to a savings goal.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GoalContributionDto>>> GetContributions(int goalId)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetContributions called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var contributions = await _goalContributionService.GetContributionsAsync(goalId, householdId);
        return Ok(contributions);
    }

    /// <summary>
    /// Gets a single contribution by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GoalContributionDto>> GetContribution(int goalId, int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetContribution called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var contribution = await _goalContributionService.GetContributionAsync(id, householdId);
        if (contribution == null || contribution.GoalId != goalId)
        {
            return NotFound(new { message = "Contribution not found" });
        }

        return Ok(contribution);
    }

    /// <summary>
    /// Creates a new contribution to a goal.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GoalContributionDto>> CreateContribution(int goalId, [FromBody] CreateGoalContributionDto request)
    {
        var householdId = User.GetHouseholdId();
        var userId = User.GetUserId();

        if (householdId <= 0 || userId <= 0)
        {
            _logger.LogWarning("CreateContribution called with invalid user context.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        request.GoalId = goalId;
        var contribution = await _goalContributionService.CreateContributionAsync(householdId, userId, request);
        return CreatedAtAction(nameof(GetContribution), new { goalId, id = contribution.Id }, contribution);
    }

    /// <summary>
    /// Updates a contribution.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<GoalContributionDto>> UpdateContribution(int goalId, int id, [FromBody] UpdateGoalContributionDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("UpdateContribution called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var existingContribution = await _goalContributionService.GetContributionAsync(id, householdId);
        if (existingContribution == null || existingContribution.GoalId != goalId)
        {
            return NotFound(new { message = "Contribution not found" });
        }

        var contribution = await _goalContributionService.UpdateContributionAsync(id, householdId, request);
        return Ok(contribution);
    }

    /// <summary>
    /// Deletes a contribution.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContribution(int goalId, int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("DeleteContribution called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var existingContribution = await _goalContributionService.GetContributionAsync(id, householdId);
        if (existingContribution == null || existingContribution.GoalId != goalId)
        {
            return NotFound(new { message = "Contribution not found" });
        }

        await _goalContributionService.DeleteContributionAsync(id, householdId);
        return NoContent();
    }
}
