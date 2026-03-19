using HouseholdBudgetApi.DTOs.Budget;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Budgets controller for managing monthly category budgets.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(IBudgetService budgetService, ILogger<BudgetsController> logger)
    {
        _budgetService = budgetService;
        _logger = logger;
    }

    /// <summary>
    /// Gets budgets for a specific month.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetBudgets([FromQuery] int year, [FromQuery] int month)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetBudgets called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        if (month is < 1 or > 12)
        {
            return BadRequest(new { message = "Month must be between 1 and 12." });
        }

        var budgets = await _budgetService.GetBudgetsAsync(householdId, new DateTime(year, month, 1));
        return Ok(budgets);
    }

    /// <summary>
    /// Gets a single budget by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetDto>> GetBudget(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetBudget called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var budget = await _budgetService.GetBudgetAsync(id, householdId);
        if (budget == null)
        {
            return NotFound(new { message = "Budget not found" });
        }

        return Ok(budget);
    }

    /// <summary>
    /// Creates a new budget.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BudgetDto>> CreateBudget([FromBody] CreateBudgetDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("CreateBudget called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var budget = await _budgetService.CreateBudgetAsync(householdId, request);
        return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
    }

    /// <summary>
    /// Updates a budget.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetDto>> UpdateBudget(int id, [FromBody] UpdateBudgetDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("UpdateBudget called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var budget = await _budgetService.UpdateBudgetAsync(id, householdId, request);
        return Ok(budget);
    }

    /// <summary>
    /// Deletes a budget.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("DeleteBudget called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        await _budgetService.DeleteBudgetAsync(id, householdId);
        return NoContent();
    }
}
