using HouseholdBudgetApi.DTOs.Dashboard;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Dashboard service interface for providing summary data.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets dashboard summary for a household for a specific month.
    /// </summary>
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(int householdId, int year, int month);
}
