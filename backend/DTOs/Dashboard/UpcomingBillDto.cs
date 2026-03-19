namespace HouseholdBudgetApi.DTOs.Dashboard;

/// <summary>
/// DTO for upcoming bills.
/// </summary>
public class UpcomingBillDto
{
    /// <summary>
    /// Bill name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Bill amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Due date.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Days until due.
    /// </summary>
    public int DaysUntilDue => (int)(DueDate - DateTime.UtcNow).TotalDays;

    /// <summary>
    /// Whether the bill is paid.
    /// </summary>
    public bool IsPaid { get; set; }
}
