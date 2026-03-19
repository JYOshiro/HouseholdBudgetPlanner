using HouseholdBudgetApi.DTOs.GoalContribution;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Goal Contribution service interface for managing contributions to savings goals.
/// </summary>
public interface IGoalContributionService
{
    /// <summary>
    /// Gets all contributions to a savings goal.
    /// </summary>
    Task<IEnumerable<GoalContributionDto>> GetContributionsAsync(int goalId, int householdId);

    /// <summary>
    /// Gets a single contribution by ID.
    /// </summary>
    Task<GoalContributionDto?> GetContributionAsync(int contributionId, int householdId);

    /// <summary>
    /// Creates a new contribution to a goal.
    /// </summary>
    Task<GoalContributionDto> CreateContributionAsync(int householdId, int userId, CreateGoalContributionDto request);

    /// <summary>
    /// Updates a contribution.
    /// </summary>
    Task<GoalContributionDto> UpdateContributionAsync(int contributionId, int householdId, UpdateGoalContributionDto request);

    /// <summary>
    /// Deletes a contribution.
    /// </summary>
    Task DeleteContributionAsync(int contributionId, int householdId);
}
