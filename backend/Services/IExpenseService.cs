using HouseholdBudgetApi.DTOs.Expense;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Expense service interface for managing expense records.
/// </summary>
public interface IExpenseService
{
    /// <summary>
    /// Gets expenses for a household with optional filtering.
    /// </summary>
    Task<IEnumerable<ExpenseDto>> GetExpensesAsync(int householdId, ExpenseFilterDto? filter = null);

    /// <summary>
    /// Gets a single expense by ID.
    /// </summary>
    Task<ExpenseDto?> GetExpenseAsync(int expenseId, int householdId);

    /// <summary>
    /// Creates a new expense.
    /// </summary>
    Task<ExpenseDto> CreateExpenseAsync(int householdId, int userId, CreateExpenseDto request);

    /// <summary>
    /// Updates an expense.
    /// </summary>
    Task<ExpenseDto> UpdateExpenseAsync(int expenseId, int householdId, UpdateExpenseDto request);

    /// <summary>
    /// Deletes an expense.
    /// </summary>
    Task DeleteExpenseAsync(int expenseId, int householdId);

    /// <summary>
    /// Gets total expenses for a household in a specific month.
    /// </summary>
    Task<decimal> GetMonthlyTotalAsync(int householdId, DateTime month);

    /// <summary>
    /// Gets total spent by category for a household in a specific month.
    /// </summary>
    Task<Dictionary<int, decimal>> GetCategorySpendingAsync(int householdId, DateTime month);
}
