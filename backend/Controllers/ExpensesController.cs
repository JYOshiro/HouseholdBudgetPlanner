using HouseholdBudgetApi.DTOs.Expense;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Expenses controller for managing expense records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IExpenseService expenseService, ILogger<ExpensesController> logger)
    {
        _expenseService = expenseService;
        _logger = logger;
    }

    /// <summary>
    /// Gets expenses with optional filtering.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses([FromQuery] ExpenseFilterDto? filter = null)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetExpenses called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var expenses = await _expenseService.GetExpensesAsync(householdId, filter);
        return Ok(expenses);
    }

    /// <summary>
    /// Gets a single expense by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpense(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetExpense called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var expense = await _expenseService.GetExpenseAsync(id, householdId);
        if (expense == null)
        {
            return NotFound(new { message = "Expense not found" });
        }

        return Ok(expense);
    }

    /// <summary>
    /// Creates a new expense.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] CreateExpenseDto request)
    {
        var householdId = User.GetHouseholdId();
        var userId = User.GetUserId();

        if (householdId <= 0 || userId <= 0)
        {
            _logger.LogWarning("CreateExpense called with invalid user context.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var expense = await _expenseService.CreateExpenseAsync(householdId, userId, request);
        return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
    }

    /// <summary>
    /// Updates an expense.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseDto>> UpdateExpense(int id, [FromBody] UpdateExpenseDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("UpdateExpense called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var expense = await _expenseService.UpdateExpenseAsync(id, householdId, request);
        return Ok(expense);
    }

    /// <summary>
    /// Deletes an expense.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("DeleteExpense called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        await _expenseService.DeleteExpenseAsync(id, householdId);
        return NoContent();
    }
}
