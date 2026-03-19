using HouseholdBudgetApi.DTOs.Income;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Income controller for managing income records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncomeController : ControllerBase
{
    private readonly IIncomeService _incomeService;
    private readonly ILogger<IncomeController> _logger;

    public IncomeController(IIncomeService incomeService, ILogger<IncomeController> logger)
    {
        _incomeService = incomeService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all income entries for the user's household.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncomeDto>>> GetIncome()
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetIncome called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var income = await _incomeService.GetIncomeAsync(householdId);
        return Ok(income);
    }

    /// <summary>
    /// Gets a single income entry by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<IncomeDto>> GetIncomeById(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetIncomeById called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var income = await _incomeService.GetIncomeByIdAsync(id, householdId);
        if (income == null)
        {
            return NotFound(new { message = "Income entry not found" });
        }

        return Ok(income);
    }

    /// <summary>
    /// Creates a new income entry.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<IncomeDto>> CreateIncome([FromBody] CreateIncomeDto request)
    {
        var householdId = User.GetHouseholdId();
        var userId = User.GetUserId();

        if (householdId <= 0 || userId <= 0)
        {
            _logger.LogWarning("CreateIncome called with invalid user context.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var income = await _incomeService.CreateIncomeAsync(householdId, userId, request);
        return CreatedAtAction(nameof(GetIncomeById), new { id = income.Id }, income);
    }

    /// <summary>
    /// Updates an income entry.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<IncomeDto>> UpdateIncome(int id, [FromBody] UpdateIncomeDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("UpdateIncome called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var income = await _incomeService.UpdateIncomeAsync(id, householdId, request);
        return Ok(income);
    }

    /// <summary>
    /// Deletes an income entry.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIncome(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("DeleteIncome called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        await _incomeService.DeleteIncomeAsync(id, householdId);
        return NoContent();
    }
}
