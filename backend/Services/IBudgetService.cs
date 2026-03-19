using HouseholdBudgetApi.DTOs.Budget;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Budget service interface for managing monthly category budgets.
/// </summary>
public interface IBudgetService
{
    /// <summary>
    /// Gets all budgets for a household in a specific month.
    /// </summary>
    Task<IEnumerable<BudgetDto>> GetBudgetsAsync(int householdId, DateTime month);

    /// <summary>
    /// Gets a single budget by ID.
    /// </summary>
    Task<BudgetDto?> GetBudgetAsync(int budgetId, int householdId);

    /// <summary>
    /// Creates a new budget.
    /// </summary>
    Task<BudgetDto> CreateBudgetAsync(int householdId, CreateBudgetDto request);

    /// <summary>
    /// Updates a budget.
    /// </summary>
    Task<BudgetDto> UpdateBudgetAsync(int budgetId, int householdId, UpdateBudgetDto request);

    /// <summary>
    /// Deletes a budget.
    /// </summary>
    Task DeleteBudgetAsync(int budgetId, int householdId);
}
