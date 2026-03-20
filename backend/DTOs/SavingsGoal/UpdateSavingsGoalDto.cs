namespace HouseholdBudgetApi.DTOs.SavingsGoal;

/// <summary>
/// DTO for updating a savings goal.
/// </summary>
public class UpdateSavingsGoalDto
{
    /// <summary>
    /// Name of the goal.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Target amount.
    /// </summary>
    public decimal? TargetAmount { get; set; }

    /// <summary>
    /// Target date.
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Priority level.
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// Goal status: "Active" or "Archived". Use to archive or reopen a goal.
    /// </summary>
    public string? Status { get; set; }
}
