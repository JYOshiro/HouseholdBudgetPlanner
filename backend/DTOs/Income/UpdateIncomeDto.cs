namespace HouseholdBudgetApi.DTOs.Income;

/// <summary>
/// DTO for updating income.
/// </summary>
public class UpdateIncomeDto
{
    /// <summary>
    /// Amount of income.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Source of income.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Date of the income.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public int? CategoryId { get; set; }
}
