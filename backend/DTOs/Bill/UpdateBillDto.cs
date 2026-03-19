namespace HouseholdBudgetApi.DTOs.Bill;

/// <summary>
/// DTO for updating a bill.
/// </summary>
public class UpdateBillDto
{
    /// <summary>
    /// Name of the bill.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Amount of the bill.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Frequency.
    /// </summary>
    public string? Frequency { get; set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public int? CategoryId { get; set; }
}
