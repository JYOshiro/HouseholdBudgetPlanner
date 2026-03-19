using HouseholdBudgetApi.DTOs.Income;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Income service interface for managing income records.
/// </summary>
public interface IIncomeService
{
    /// <summary>
    /// Gets all income entries for a household.
    /// </summary>
    Task<IEnumerable<IncomeDto>> GetIncomeAsync(int householdId);

    /// <summary>
    /// Gets a single income entry by ID.
    /// </summary>
    Task<IncomeDto?> GetIncomeByIdAsync(int incomeId, int householdId);

    /// <summary>
    /// Creates a new income entry.
    /// </summary>
    Task<IncomeDto> CreateIncomeAsync(int householdId, int userId, CreateIncomeDto request);

    /// <summary>
    /// Updates an income entry.
    /// </summary>
    Task<IncomeDto> UpdateIncomeAsync(int incomeId, int householdId, UpdateIncomeDto request);

    /// <summary>
    /// Deletes an income entry.
    /// </summary>
    Task DeleteIncomeAsync(int incomeId, int householdId);

    /// <summary>
    /// Gets total income for a household in a specific month.
    /// </summary>
    Task<decimal> GetMonthlyTotalAsync(int householdId, DateTime month);
}
