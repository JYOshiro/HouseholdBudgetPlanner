namespace HouseholdBudgetApi.DTOs.Bill;

/// <summary>
/// DTO for creating a bill.
/// </summary>
public class CreateBillDto
{
    /// <summary>
    /// Name of the bill.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Amount of the bill.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Due date of the bill.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Frequency: "OneTime", "Monthly", "Quarterly", "Annual".
    /// </summary>
    public string Frequency { get; set; } = "Monthly";

    /// <summary>
    /// Category ID.
    /// </summary>
    public int CategoryId { get; set; }
}
