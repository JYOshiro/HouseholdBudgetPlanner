namespace HouseholdBudgetApi.DTOs.GoalContribution;

/// <summary>
/// DTO for creating a goal contribution.
/// </summary>
public class CreateGoalContributionDto
{
    /// <summary>
    /// Amount to contribute.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Date of contribution.
    /// </summary>
    public DateTime ContributionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Savings goal ID.
    /// </summary>
    public int GoalId { get; set; }
}
