namespace HouseholdBudgetApi.DTOs.Expense;

/// <summary>
/// DTO for updating an expense.
/// </summary>
public class UpdateExpenseDto
{
    /// <summary>
    /// Amount of the expense.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Description of the expense.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether this expense is shared.
    /// </summary>
    public bool? IsShared { get; set; }

    /// <summary>
    /// Date of the expense.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public int? CategoryId { get; set; }
}
