namespace HouseholdBudgetApi.DTOs.Expense;

/// <summary>
/// DTO for creating a new expense.
/// </summary>
public class CreateExpenseDto
{
    /// <summary>
    /// Amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Description of the expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether this expense is shared among household members.
    /// </summary>
    public bool IsShared { get; set; }

    /// <summary>
    /// Date of the expense.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Category ID.
    /// </summary>
    public int CategoryId { get; set; }
}
