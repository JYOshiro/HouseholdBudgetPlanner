namespace HouseholdBudgetApi.DTOs.GoalContribution;

/// <summary>
/// DTO for goal contribution information.
/// </summary>
public class GoalContributionDto
{
    /// <summary>
    /// Contribution ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Amount contributed.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Date of contribution.
    /// </summary>
    public DateTime ContributionDate { get; set; }

    /// <summary>
    /// Savings goal ID.
    /// </summary>
    public int GoalId { get; set; }

    /// <summary>
    /// User ID of contributor.
    /// </summary>
    public int ContributedByUserId { get; set; }

    /// <summary>
    /// Name of contributor.
    /// </summary>
    public string ContributedByUserName { get; set; } = string.Empty;
}
