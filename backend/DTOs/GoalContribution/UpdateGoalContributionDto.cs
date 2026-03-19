namespace HouseholdBudgetApi.DTOs.GoalContribution;

/// <summary>
/// DTO for updating a goal contribution.
/// </summary>
public class UpdateGoalContributionDto
{
    /// <summary>
    /// Amount.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Contribution date.
    /// </summary>
    public DateTime? ContributionDate { get; set; }
}
