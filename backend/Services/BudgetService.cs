using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Budget;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Budget service implementation for managing monthly budgets.
/// </summary>
public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext _context;
    private readonly IExpenseService _expenseService;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(ApplicationDbContext context, IExpenseService expenseService, ILogger<BudgetService> logger)
    {
        _context = context;
        _expenseService = expenseService;
        _logger = logger;
    }

    public async Task<IEnumerable<BudgetDto>> GetBudgetsAsync(int householdId, DateTime month)
    {
        try
        {
            var query = _context.Budgets
                .Where(b => b.HouseholdId == householdId && 
                    b.Month.Year == month.Year && b.Month.Month == month.Month)
                .Include(b => b.Category)
                .AsNoTracking();

            var budgets = await query.ToListAsync();
            var spending = await _expenseService.GetCategorySpendingAsync(householdId, month);

            return budgets.Select(b => 
            {
                var spent = spending.ContainsKey(b.CategoryId) ? spending[b.CategoryId] : 0;
                var percentageUsed = b.Amount > 0 ? (spent / b.Amount * 100) : 0;

                return new BudgetDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    Month = b.Month,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category?.Name ?? "Uncategorized",
                    CurrentSpent = spent
                };
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting budgets for household {householdId}: {ex.Message}");
            throw;
        }
    }

    public async Task<BudgetDto?> GetBudgetAsync(int budgetId, int householdId)
    {
        try
        {
            var budget = await _context.Budgets
                .Where(b => b.Id == budgetId && b.HouseholdId == householdId)
                .Include(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (budget == null)
                return null;

            var spending = await _expenseService.GetCategorySpendingAsync(householdId, budget.Month);
            var spent = spending.ContainsKey(budget.CategoryId) ? spending[budget.CategoryId] : 0;
            var percentageUsed = budget.Amount > 0 ? (spent / budget.Amount * 100) : 0;

            return new BudgetDto
            {
                Id = budget.Id,
                Amount = budget.Amount,
                Month = budget.Month,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Uncategorized",
                CurrentSpent = spent
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting budget {budgetId}: {ex.Message}");
            throw;
        }
    }

    public async Task<BudgetDto> CreateBudgetAsync(int householdId, CreateBudgetDto request)
    {
        try
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Budget amount must be greater than zero.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && 
                    (c.IsSystemDefault || c.HouseholdId == householdId));

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            var monthDate = new DateTime(request.Month.Year, request.Month.Month, 1);

            // Check for duplicate budget (one per household/category/month)
            var existingBudget = await _context.Budgets
                .FirstOrDefaultAsync(b => 
                    b.HouseholdId == householdId && 
                    b.CategoryId == request.CategoryId &&
                    b.Month.Year == monthDate.Year &&
                    b.Month.Month == monthDate.Month);

            if (existingBudget != null)
                throw new InvalidOperationException("A budget already exists for this category in this month.");

            var budget = new Budget
            {
                Amount = request.Amount,
                Month = monthDate,
                CategoryId = request.CategoryId,
                HouseholdId = householdId
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Budget created: {category.Name} for {monthDate:yyyy-MM-dd}");

            return new BudgetDto
            {
                Id = budget.Id,
                Amount = budget.Amount,
                Month = budget.Month,
                CategoryId = budget.CategoryId,
                CategoryName = category.Name,
                CurrentSpent = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating budget: {ex.Message}");
            throw;
        }
    }

    public async Task<BudgetDto> UpdateBudgetAsync(int budgetId, int householdId, UpdateBudgetDto request)
    {
        try
        {
            var budget = await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == budgetId && b.HouseholdId == householdId);

            if (budget == null)
                throw new KeyNotFoundException($"Budget {budgetId} not found.");

            if (request.Amount.HasValue)
            {
                if (request.Amount <= 0)
                    throw new ArgumentException("Budget amount must be greater than zero.");

                budget.Amount = request.Amount.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Budget updated: {budgetId}");

            var spending = await _expenseService.GetCategorySpendingAsync(householdId, budget.Month);
            var spent = spending.ContainsKey(budget.CategoryId) ? spending[budget.CategoryId] : 0;
            var percentageUsed = budget.Amount > 0 ? (spent / budget.Amount * 100) : 0;

            return new BudgetDto
            {
                Id = budget.Id,
                Amount = budget.Amount,
                Month = budget.Month,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Uncategorized",
                CurrentSpent = spent
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating budget {budgetId}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteBudgetAsync(int budgetId, int householdId)
    {
        try
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == budgetId && b.HouseholdId == householdId);

            if (budget == null)
                throw new KeyNotFoundException($"Budget {budgetId} not found.");

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Budget deleted: {budgetId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting budget {budgetId}: {ex.Message}");
            throw;
        }
    }
}
