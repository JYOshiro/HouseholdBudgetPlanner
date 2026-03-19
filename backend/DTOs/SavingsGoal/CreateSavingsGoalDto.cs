namespace HouseholdBudgetApi.DTOs.SavingsGoal;

/// <summary>
/// DTO for creating a savings goal.
/// </summary>
public class CreateSavingsGoalDto
{
    /// <summary>
    /// Name of the goal.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Target amount to save.
    /// </summary>
    public decimal TargetAmount { get; set; }

    /// <summary>
    /// Target date to reach the goal.
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Priority level.
    /// </summary>
    public string Priority { get; set; } = "Normal";
}
