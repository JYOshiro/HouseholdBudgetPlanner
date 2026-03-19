using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Income;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Income service implementation for managing income records.
/// </summary>
public class IncomeService : IIncomeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<IncomeService> _logger;

    public IncomeService(ApplicationDbContext context, ILogger<IncomeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<IncomeDto>> GetIncomeAsync(int householdId)
    {
        try
        {
            var incomes = await _context.IncomeEntries
                .Where(i => i.HouseholdId == householdId)
                .Include(i => i.Category)
                .Include(i => i.User)
                .OrderByDescending(i => i.Date)
                .AsNoTracking()
                .ToListAsync();

            return incomes.Select(i => new IncomeDto
            {
                Id = i.Id,
                Amount = i.Amount,
                Source = i.Source,
                Date = i.Date,
                CategoryId = i.CategoryId,
                CategoryName = i.Category?.Name ?? "Uncategorized",
                UserId = i.UserId,
                UserName = i.User?.FirstName + " " + i.User?.LastName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting income for household {householdId}: {ex.Message}");
            throw;
        }
    }

    public async Task<IncomeDto?> GetIncomeByIdAsync(int incomeId, int householdId)
    {
        try
        {
            var income = await _context.IncomeEntries
                .Where(i => i.Id == incomeId && i.HouseholdId == householdId)
                .Include(i => i.Category)
                .Include(i => i.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (income == null)
                return null;

            return new IncomeDto
            {
                Id = income.Id,
                Amount = income.Amount,
                Source = income.Source,
                Date = income.Date,
                CategoryId = income.CategoryId,
                CategoryName = income.Category?.Name ?? "Uncategorized",
                UserId = income.UserId,
                UserName = income.User?.FirstName + " " + income.User?.LastName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting income {incomeId}: {ex.Message}");
            throw;
        }
    }

    public async Task<IncomeDto> CreateIncomeAsync(int householdId, int userId, CreateIncomeDto request)
    {
        try
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Income amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.Source))
                throw new ArgumentException("Income source is required.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && 
                    (c.IsSystemDefault || c.HouseholdId == householdId));

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            var income = new Income
            {
                Amount = request.Amount,
                Source = request.Source.Trim(),
                Date = request.Date,
                CategoryId = request.CategoryId,
                HouseholdId = householdId,
                UserId = userId
            };

            _context.IncomeEntries.Add(income);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Income created: {request.Source} for household {householdId}");

            return new IncomeDto
            {
                Id = income.Id,
                Amount = income.Amount,
                Source = income.Source,
                Date = income.Date,
                CategoryId = income.CategoryId,
                CategoryName = category.Name,
                UserId = income.UserId,
                UserName = ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating income: {ex.Message}");
            throw;
        }
    }

    public async Task<IncomeDto> UpdateIncomeAsync(int incomeId, int householdId, UpdateIncomeDto request)
    {
        try
        {
            var income = await _context.IncomeEntries
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == incomeId && i.HouseholdId == householdId);

            if (income == null)
                throw new KeyNotFoundException($"Income {incomeId} not found.");

            if (request.Amount.HasValue && request.Amount <= 0)
                throw new ArgumentException("Income amount must be greater than zero.");

            if (request.Amount.HasValue)
                income.Amount = request.Amount.Value;

            if (!string.IsNullOrWhiteSpace(request.Source))
                income.Source = request.Source.Trim();

            if (request.Date.HasValue)
                income.Date = request.Date.Value;

            if (request.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId && 
                        (c.IsSystemDefault || c.HouseholdId == householdId));

                if (category == null)
                    throw new KeyNotFoundException("Category not found.");

                income.CategoryId = request.CategoryId.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Income updated: {incomeId}");

            return new IncomeDto
            {
                Id = income.Id,
                Amount = income.Amount,
                Source = income.Source,
                Date = income.Date,
                CategoryId = income.CategoryId,
                CategoryName = income.Category?.Name ?? "Uncategorized",
                UserId = income.UserId,
                UserName = ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating income {incomeId}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteIncomeAsync(int incomeId, int householdId)
    {
        try
        {
            var income = await _context.IncomeEntries
                .FirstOrDefaultAsync(i => i.Id == incomeId && i.HouseholdId == householdId);

            if (income == null)
                throw new KeyNotFoundException($"Income {incomeId} not found.");

            _context.IncomeEntries.Remove(income);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Income deleted: {incomeId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting income {incomeId}: {ex.Message}");
            throw;
        }
    }

    public async Task<decimal> GetMonthlyTotalAsync(int householdId, DateTime month)
    {
        try
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var total = await _context.IncomeEntries
                .Where(i => i.HouseholdId == householdId && 
                    i.Date >= startDate && i.Date <= endDate)
                .SumAsync(i => i.Amount);

            return total;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting monthly income for household {householdId}: {ex.Message}");
            throw;
        }
    }
}
