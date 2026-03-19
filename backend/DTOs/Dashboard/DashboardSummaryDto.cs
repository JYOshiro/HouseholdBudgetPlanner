namespace HouseholdBudgetApi.DTOs.Dashboard;

/// <summary>
/// DTO for dashboard summary data.
/// </summary>
public class DashboardSummaryDto
{
    /// <summary>
    /// Total income for the selected month.
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Total expenses for the selected month.
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Net (income - expenses).
    /// </summary>
    public decimal NetAmount => TotalIncome - TotalExpenses;

    /// <summary>
    /// Budget usage by category for the month.
    /// </summary>
    public ICollection<BudgetUsageDto> BudgetUsage { get; set; } = new List<BudgetUsageDto>();

    /// <summary>
    /// Upcoming bills in the next 30 days.
    /// </summary>
    public ICollection<UpcomingBillDto> UpcomingBills { get; set; } = new List<UpcomingBillDto>();

    /// <summary>
    /// Recent transactions (last 10).
    /// </summary>
    public ICollection<RecentTransactionDto> RecentTransactions { get; set; } = new List<RecentTransactionDto>();

    /// <summary>
    /// Savings goal progress.
    /// </summary>
    public ICollection<SavingsProgressDto> SavingsProgress { get; set; } = new List<SavingsProgressDto>();
}
