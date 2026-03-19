namespace HouseholdBudgetApi.DTOs.Income;

/// <summary>
/// DTO for creating income.
/// </summary>
public class CreateIncomeDto
{
    /// <summary>
    /// Amount of income.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Source of income.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Date of the income.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Category ID.
    /// </summary>
    public int CategoryId { get; set; }
}
