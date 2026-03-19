using HouseholdBudgetApi.DTOs.Bill;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Bill service interface for managing recurring and one-time bills.
/// </summary>
public interface IBillService
{
    /// <summary>
    /// Gets all bills for a household.
    /// </summary>
    Task<IEnumerable<BillDto>> GetBillsAsync(int householdId);

    /// <summary>
    /// Gets upcoming bills (due within next 30 days).
    /// </summary>
    Task<IEnumerable<BillDto>> GetUpcomingBillsAsync(int householdId);

    /// <summary>
    /// Gets a single bill by ID.
    /// </summary>
    Task<BillDto?> GetBillAsync(int billId, int householdId);

    /// <summary>
    /// Creates a new bill.
    /// </summary>
    Task<BillDto> CreateBillAsync(int householdId, CreateBillDto request);

    /// <summary>
    /// Updates a bill.
    /// </summary>
    Task<BillDto> UpdateBillAsync(int billId, int householdId, UpdateBillDto request);

    /// <summary>
    /// Deletes a bill.
    /// </summary>
    Task DeleteBillAsync(int billId, int householdId);

    /// <summary>
    /// Marks a bill as paid.
    /// </summary>
    Task<BillDto> MarkBillAsPaidAsync(int billId, int householdId, MarkBillPaidDto request);
}
