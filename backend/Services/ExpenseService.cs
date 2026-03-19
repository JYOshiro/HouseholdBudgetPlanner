using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Expense;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Expense service implementation for managing expense records.
/// </summary>
public class ExpenseService : IExpenseService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(ApplicationDbContext context, ILogger<ExpenseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets expenses for a household with optional filtering.
    /// </summary>
    public async Task<IEnumerable<ExpenseDto>> GetExpensesAsync(int householdId, ExpenseFilterDto? filter = null)
    {
        try
        {
            var pageNumber = Math.Max(filter?.PageNumber ?? 1, 1);
            var pageSize = Math.Max(filter?.PageSize ?? 20, 1);

            var query = _context.Expenses
                .Where(e => e.HouseholdId == householdId)
                .Include(e => e.Category)
                .Include(e => e.PaidByUser)
                .AsNoTracking();

            // Apply filters
            if (filter != null)
            {
                if (filter.CategoryId.HasValue && filter.CategoryId > 0)
                    query = query.Where(e => e.CategoryId == filter.CategoryId);

                if (filter.PaidByUserId.HasValue && filter.PaidByUserId > 0)
                    query = query.Where(e => e.PaidByUserId == filter.PaidByUserId);

                if (filter.StartDate.HasValue)
                    query = query.Where(e => e.Date >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(e => e.Date <= filter.EndDate.Value);

                if (filter.IsShared.HasValue)
                    query = query.Where(e => e.IsShared == filter.IsShared);
            }

            var expenses = await query
                .OrderByDescending(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                IsShared = e.IsShared,
                Date = e.Date,
                CategoryId = e.CategoryId,
                CategoryName = e.Category?.Name ?? "Uncategorized",
                PaidByUserId = e.PaidByUserId,
                PaidByUserName = e.PaidByUser?.FirstName + " " + e.PaidByUser?.LastName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting expenses for household {householdId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets a single expense by ID.
    /// </summary>
    public async Task<ExpenseDto?> GetExpenseAsync(int expenseId, int householdId)
    {
        try
        {
            var expense = await _context.Expenses
                .Where(e => e.Id == expenseId && e.HouseholdId == householdId)
                .Include(e => e.Category)
                .Include(e => e.PaidByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (expense == null)
                return null;

            return new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                IsShared = expense.IsShared,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category?.Name ?? "Uncategorized",
                PaidByUserId = expense.PaidByUserId,
                PaidByUserName = expense.PaidByUser?.FirstName + " " + expense.PaidByUser?.LastName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting expense {expenseId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Creates a new expense.
    /// </summary>
    public async Task<ExpenseDto> CreateExpenseAsync(int householdId, int userId, CreateExpenseDto request)
    {
        try
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Expense amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Expense description is required.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && 
                    (c.IsSystemDefault || c.HouseholdId == householdId));

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            var expense = new Expense
            {
                Amount = request.Amount,
                Description = request.Description.Trim(),
                IsShared = request.IsShared,
                Date = request.Date,
                CategoryId = request.CategoryId,
                HouseholdId = householdId,
                PaidByUserId = userId
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Expense created: {request.Description} for household {householdId}");

            return new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                IsShared = expense.IsShared,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                CategoryName = category.Name,
                PaidByUserId = expense.PaidByUserId,
                PaidByUserName = ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating expense: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates an expense.
    /// </summary>
    public async Task<ExpenseDto> UpdateExpenseAsync(int expenseId, int householdId, UpdateExpenseDto request)
    {
        try
        {
            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.HouseholdId == householdId);

            if (expense == null)
                throw new KeyNotFoundException($"Expense {expenseId} not found.");

            if (request.Amount.HasValue && request.Amount <= 0)
                throw new ArgumentException("Expense amount must be greater than zero.");

            if (request.Amount.HasValue)
                expense.Amount = request.Amount.Value;

            if (!string.IsNullOrWhiteSpace(request.Description))
                expense.Description = request.Description.Trim();

            if (request.IsShared.HasValue)
                expense.IsShared = request.IsShared.Value;

            if (request.Date.HasValue)
                expense.Date = request.Date.Value;

            if (request.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId && 
                        (c.IsSystemDefault || c.HouseholdId == householdId));

                if (category == null)
                    throw new KeyNotFoundException("Category not found.");

                expense.CategoryId = request.CategoryId.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Expense updated: {expense.Id}");

            return new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                IsShared = expense.IsShared,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category?.Name ?? "Uncategorized",
                PaidByUserId = expense.PaidByUserId,
                PaidByUserName = ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating expense {expenseId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes an expense.
    /// </summary>
    public async Task DeleteExpenseAsync(int expenseId, int householdId)
    {
        try
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.HouseholdId == householdId);

            if (expense == null)
                throw new KeyNotFoundException($"Expense {expenseId} not found.");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Expense deleted: {expenseId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting expense {expenseId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets total expenses for a household in a specific month.
    /// </summary>
    public async Task<decimal> GetMonthlyTotalAsync(int householdId, DateTime month)
    {
        try
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var total = await _context.Expenses
                .Where(e => e.HouseholdId == householdId && 
                    e.Date >= startDate && e.Date <= endDate)
                .SumAsync(e => e.Amount);

            return total;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting monthly total for household {householdId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets total spent by category for a household in a specific month.
    /// </summary>
    public async Task<Dictionary<int, decimal>> GetCategorySpendingAsync(int householdId, DateTime month)
    {
        try
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var categorySpending = await _context.Expenses
                .Where(e => e.HouseholdId == householdId && 
                    e.Date >= startDate && e.Date <= endDate)
                .GroupBy(e => e.CategoryId)
                .Select(g => new { CategoryId = g.Key, Total = g.Sum(e => e.Amount) })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Total);

            return categorySpending;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting category spending for household {householdId}: {ex.Message}");
            throw;
        }
    }
}
