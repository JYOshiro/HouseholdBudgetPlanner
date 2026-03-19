namespace HouseholdBudgetApi.DTOs.Budget;

/// <summary>
/// DTO for budget information.
/// </summary>
public class BudgetDto
{
    /// <summary>
    /// Budget ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Budgeted amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Month for this budget (YYYY-MM).
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Current spent amount for this month/category.
    /// </summary>
    public decimal CurrentSpent { get; set; }

    /// <summary>
    /// Remaining budget amount.
    /// </summary>
    public decimal Remaining => Amount - CurrentSpent;

    /// <summary>
    /// Percentage used (0-100).
    /// </summary>
    public decimal PercentageUsed => Amount > 0 ? (CurrentSpent / Amount) * 100 : 0;
}
