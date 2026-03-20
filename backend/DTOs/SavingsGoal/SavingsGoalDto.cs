namespace HouseholdBudgetApi.DTOs.SavingsGoal;

/// <summary>
/// DTO for savings goal information.
/// </summary>
public class SavingsGoalDto
{
    /// <summary>
    /// Goal ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the goal.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Target amount to save.
    /// </summary>
    public decimal TargetAmount { get; set; }

    /// <summary>
    /// Current amount saved.
    /// </summary>
    public decimal CurrentAmount { get; set; }

    /// <summary>
    /// Target date to reach the goal.
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Priority: "High", "Normal", "Low".
    /// </summary>
    public string Priority { get; set; } = "Normal";

    /// <summary>
    /// Goal status: "Active", "Completed", or "Archived".
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Date the goal was completed (null if not yet completed).
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Percentage of goal completed (0-100), capped at 100.
    /// </summary>
    public decimal PercentageComplete => TargetAmount > 0 ? Math.Min(100, (CurrentAmount / TargetAmount) * 100) : 0;

    /// <summary>
    /// Remaining amount to reach the goal. Never negative.
    /// </summary>
    public decimal Remaining => Math.Max(0, TargetAmount - CurrentAmount);

    /// <summary>
    /// Whether the goal has been completed.
    /// </summary>
    public bool IsCompleted => Status == "Completed";
}
