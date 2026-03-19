using HouseholdBudgetApi.DTOs.SavingsGoal;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Savings Goal service interface for managing savings goals.
/// </summary>
public interface ISavingsGoalService
{
    /// <summary>
    /// Gets all savings goals for a household.
    /// </summary>
    Task<IEnumerable<SavingsGoalDto>> GetGoalsAsync(int householdId);

    /// <summary>
    /// Gets a single savings goal by ID.
    /// </summary>
    Task<SavingsGoalDto?> GetGoalAsync(int goalId, int householdId);

    /// <summary>
    /// Creates a new savings goal.
    /// </summary>
    Task<SavingsGoalDto> CreateGoalAsync(int householdId, CreateSavingsGoalDto request);

    /// <summary>
    /// Updates a savings goal.
    /// </summary>
    Task<SavingsGoalDto> UpdateGoalAsync(int goalId, int householdId, UpdateSavingsGoalDto request);

    /// <summary>
    /// Deletes a savings goal.
    /// </summary>
    Task DeleteGoalAsync(int goalId, int householdId);
}
