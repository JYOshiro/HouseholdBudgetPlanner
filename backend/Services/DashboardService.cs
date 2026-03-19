using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Dashboard service implementation.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
        private readonly IExpenseService _expenseService;
        private readonly IIncomeService _incomeService;
        private readonly IBudgetService _budgetService;
        private readonly IBillService _billService;
        private readonly ISavingsGoalService _goalService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        ApplicationDbContext context,
        IExpenseService expenseService,
        IIncomeService incomeService,
        IBudgetService budgetService,
        IBillService billService,
        ISavingsGoalService goalService,
        ILogger<DashboardService> logger)
    {
        _context = context;
            _expenseService = expenseService;
            _incomeService = incomeService;
            _budgetService = budgetService;
            _billService = billService;
            _goalService = goalService;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int householdId, int year, int month)
    {
        try
        {
            var targetDate = new DateTime(year, month, 1);

            var totalIncome = await _incomeService.GetMonthlyTotalAsync(householdId, targetDate);
            var totalExpenses = await _expenseService.GetMonthlyTotalAsync(householdId, targetDate);
            var budgets = await _budgetService.GetBudgetsAsync(householdId, targetDate);
            var upcomingBills = await _billService.GetUpcomingBillsAsync(householdId);
            var savingsGoals = await _goalService.GetGoalsAsync(householdId);

            var recentExpenses = await _context.Expenses
                .Where(e => e.HouseholdId == householdId)
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            var recentIncome = await _context.IncomeEntries
                .Where(i => i.HouseholdId == householdId)
                .Include(i => i.Category)
                .OrderByDescending(i => i.Date)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            var recentTransactions = new List<RecentTransactionDto>();

            foreach (var expense in recentExpenses)
            {
                recentTransactions.Add(new RecentTransactionDto
                {
                    Id = expense.Id,
                    Description = expense.Description,
                    Type = "Expense",
                    Amount = expense.Amount,
                    Category = expense.Category?.Name ?? "Uncategorized",
                    Date = expense.Date
                });
            }

            foreach (var income in recentIncome)
            {
                recentTransactions.Add(new RecentTransactionDto
                {
                    Id = income.Id,
                    Description = income.Source,
                    Type = "Income",
                    Amount = income.Amount,
                    Category = income.Category?.Name ?? "Uncategorized",
                    Date = income.Date
                });
            }

            recentTransactions = recentTransactions.OrderByDescending(t => t.Date).Take(10).ToList();

            _logger.LogInformation($"Dashboard summary retrieved for household {householdId} for {year}-{month:D2}");

            return new DashboardSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                BudgetUsage = budgets.Select(b => new BudgetUsageDto
                {
                    CategoryName = b.CategoryName,
                    BudgetAmount = b.Amount,
                    Spent = b.CurrentSpent
                }).ToList(),
                UpcomingBills = upcomingBills.Select(b => new UpcomingBillDto
                {
                    Name = b.Name,
                    Amount = b.Amount,
                    DueDate = b.DueDate,
                    IsPaid = b.IsPaid
                }).ToList(),
                RecentTransactions = recentTransactions,
                SavingsProgress = savingsGoals.Select(g => new SavingsProgressDto
                {
                    GoalName = g.Name,
                    TargetAmount = g.TargetAmount,
                    CurrentAmount = g.CurrentAmount,
                    Priority = g.Priority
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting dashboard summary for household {householdId}: {ex.Message}");
            throw;
        }
    }
}
