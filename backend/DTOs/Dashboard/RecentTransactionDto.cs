namespace HouseholdBudgetApi.DTOs.Dashboard;

/// <summary>
/// DTO for recent transactions.
/// </summary>
public class RecentTransactionDto
{
    /// <summary>
    /// Transaction ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Transaction description/name.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Transaction type: "Expense" or "Income".
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Transaction date.
    /// </summary>
    public DateTime Date { get; set; }
}
