namespace HouseholdBudgetApi.DTOs.Budget;

/// <summary>
/// DTO for updating a budget.
/// </summary>
public class UpdateBudgetDto
{
    /// <summary>
    /// Budget amount.
    /// </summary>
    public decimal? Amount { get; set; }
}
