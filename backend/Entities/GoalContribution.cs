namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents a contribution to a savings goal.
/// </summary>
public class GoalContribution : BaseEntity
{
    /// <summary>
    /// Amount contributed to the goal.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Date of the contribution.
    /// </summary>
    public DateTime ContributionDate { get; set; }

    /// <summary>
    /// Foreign key to the savings goal.
    /// </summary>
    public int GoalId { get; set; }

    /// <summary>
    /// Foreign key to the user who made the contribution.
    /// </summary>
    public int ContributedByUserId { get; set; }

    /// <summary>
    /// Navigation property to the savings goal.
    /// </summary>
    public virtual SavingsGoal? Goal { get; set; }

    /// <summary>
    /// Navigation property to the user who contributed.
    /// </summary>
    public virtual User? ContributedByUser { get; set; }
}
