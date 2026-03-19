namespace HouseholdBudgetApi.DTOs.Dashboard;

/// <summary>
/// DTO for budget usage by category.
/// </summary>
public class BudgetUsageDto
{
    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Budgeted amount.
    /// </summary>
    public decimal BudgetAmount { get; set; }

    /// <summary>
    /// Current spent amount.
    /// </summary>
    public decimal Spent { get; set; }

    /// <summary>
    /// Percentage of budget used (0-100).
    /// </summary>
    public decimal PercentageUsed => BudgetAmount > 0 ? (Spent / BudgetAmount) * 100 : 0;

    /// <summary>
    /// Remaining budget.
    /// </summary>
    public decimal Remaining => BudgetAmount - Spent;
}
