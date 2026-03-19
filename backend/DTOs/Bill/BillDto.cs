namespace HouseholdBudgetApi.DTOs.Bill;

/// <summary>
/// DTO for bill information.
/// </summary>
public class BillDto
{
    /// <summary>
    /// Bill ID.
    /// </summary>
    public int Id { get; set; }

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
    /// Indicates if the bill is paid.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Last date the bill was paid.
    /// </summary>
    public DateTime? LastPaidDate { get; set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Days until due date (negative if overdue).
    /// </summary>
    public int DaysUntilDue => (int)(DueDate - DateTime.UtcNow).TotalDays;
}
