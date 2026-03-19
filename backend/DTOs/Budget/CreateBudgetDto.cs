namespace HouseholdBudgetApi.DTOs.Budget;

/// <summary>
/// DTO for creating a budget.
/// </summary>
public class CreateBudgetDto
{
    /// <summary>
    /// Budget amount for the month.
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
}
