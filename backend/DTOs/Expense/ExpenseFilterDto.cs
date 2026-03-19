namespace HouseholdBudgetApi.DTOs.Expense;

/// <summary>
/// DTO for filtering expenses.
/// </summary>
public class ExpenseFilterDto
{
    /// <summary>
    /// Filter by category ID.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filter by user who paid.
    /// </summary>
    public int? PaidByUserId { get; set; }

    /// <summary>
    /// Filter by start date.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter by end date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Filter by shared flag.
    /// </summary>
    public bool? IsShared { get; set; }

    /// <summary>
    /// Page number for pagination (default 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size for pagination (default 20).
    /// </summary>
    public int PageSize { get; set; } = 20;
}
