namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents a savings goal for the household.
/// </summary>
public class SavingsGoal : BaseEntity
{
    /// <summary>
    /// Name of the savings goal (e.g., "Vacation", "Emergency Fund").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Target amount to save.
    /// </summary>
    public decimal TargetAmount { get; set; }

    /// <summary>
    /// Current amount saved towards this goal.
    /// </summary>
    public decimal CurrentAmount { get; set; }

    /// <summary>
    /// Target date to reach the goal.
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Priority level: "High", "Normal", "Low".
    /// </summary>
    public string Priority { get; set; } = "Normal";

    /// <summary>
    /// Foreign key to the household.
    /// </summary>
    public int HouseholdId { get; set; }

    /// <summary>
    /// Navigation property to the household.
    /// </summary>
    public virtual Household? Household { get; set; }

    /// <summary>
    /// Goal status: "Active", "Completed", or "Archived".
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Date the goal was completed (null if not yet completed).
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Navigation property to all contributions to this goal.
    /// </summary>
    public virtual ICollection<GoalContribution> Contributions { get; set; } = new List<GoalContribution>();
}
