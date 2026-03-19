namespace HouseholdBudgetApi.DTOs.Expense;

/// <summary>
/// DTO for expense information.
/// </summary>
public class ExpenseDto
{
    /// <summary>
    /// Expense ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Description of the expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this expense is shared.
    /// </summary>
    public bool IsShared { get; set; }

    /// <summary>
    /// Date of the expense.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// User ID of who paid the expense.
    /// </summary>
    public int PaidByUserId { get; set; }

    /// <summary>
    /// Name of the user who paid.
    /// </summary>
    public string PaidByUserName { get; set; } = string.Empty;
}
