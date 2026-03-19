namespace HouseholdBudgetApi.DTOs.Income;

/// <summary>
/// DTO for income entry information.
/// </summary>
public class IncomeDto
{
    /// <summary>
    /// Income entry ID.
    /// </summary>
    public int Id { get; set; }

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
    /// User ID who recorded the income.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Name of the user who recorded.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
}
