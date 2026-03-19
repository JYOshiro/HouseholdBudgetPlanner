namespace HouseholdBudgetApi.DTOs.Bill;

/// <summary>
/// DTO for marking a bill as paid.
/// </summary>
public class MarkBillPaidDto
{
    /// <summary>
    /// Date when the bill was paid.
    /// </summary>
    public DateTime PaidDate { get; set; } = DateTime.UtcNow;
}
