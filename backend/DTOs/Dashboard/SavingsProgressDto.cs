namespace HouseholdBudgetApi.DTOs.Dashboard;

/// <summary>
/// DTO for savings goal progress.
/// </summary>
public class SavingsProgressDto
{
    /// <summary>
    /// Goal name.
    /// </summary>
    public string GoalName { get; set; } = string.Empty;

    /// <summary>
    /// Target amount.
    /// </summary>
    public decimal TargetAmount { get; set; }

    /// <summary>
    /// Current saved amount.
    /// </summary>
    public decimal CurrentAmount { get; set; }

    /// <summary>
    /// Percentage complete (0-100).
    /// </summary>
    public decimal PercentageComplete => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;

    /// <summary>
    /// Remaining amount to reach target.
    /// </summary>
    public decimal Remaining => TargetAmount - CurrentAmount;

    /// <summary>
    /// Priority level.
    /// </summary>
    public string Priority { get; set; } = string.Empty;
}
