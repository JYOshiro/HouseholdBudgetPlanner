using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Bill;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Bill service implementation for managing bills and recurring payments.
/// </summary>
public class BillService : IBillService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BillService> _logger;

    public BillService(ApplicationDbContext context, ILogger<BillService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<BillDto>> GetBillsAsync(int householdId)
    {
        try
        {
            var bills = await _context.Bills
                .Where(b => b.HouseholdId == householdId)
                .Include(b => b.Category)
                .OrderBy(b => b.DueDate)
                .AsNoTracking()
                .ToListAsync();

            return bills.Select(b => new BillDto
            {
                Id = b.Id,
                Name = b.Name,
                Amount = b.Amount,
                DueDate = b.DueDate,
                Frequency = b.Frequency,
                IsPaid = b.IsPaid,
                LastPaidDate = b.LastPaidDate,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.Name ?? "Uncategorized"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting bills for household {householdId}: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<BillDto>> GetUpcomingBillsAsync(int householdId)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var thirtyDaysFromNow = today.AddDays(30);

            var upcomingBills = await _context.Bills
                .Where(b => b.HouseholdId == householdId && 
                    b.DueDate >= today && b.DueDate <= thirtyDaysFromNow &&
                    !b.IsPaid)
                .Include(b => b.Category)
                .OrderBy(b => b.DueDate)
                .AsNoTracking()
                .ToListAsync();

            return upcomingBills.Select(b => new BillDto
            {
                Id = b.Id,
                Name = b.Name,
                Amount = b.Amount,
                DueDate = b.DueDate,
                Frequency = b.Frequency,
                IsPaid = b.IsPaid,
                LastPaidDate = b.LastPaidDate,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.Name ?? "Uncategorized"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting upcoming bills for household {householdId}: {ex.Message}");
            throw;
        }
    }

    public async Task<BillDto?> GetBillAsync(int billId, int householdId)
    {
        try
        {
            var bill = await _context.Bills
                .Where(b => b.Id == billId && b.HouseholdId == householdId)
                .Include(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (bill == null)
                return null;

            return new BillDto
            {
                Id = bill.Id,
                Name = bill.Name,
                Amount = bill.Amount,
                DueDate = bill.DueDate,
                Frequency = bill.Frequency,
                IsPaid = bill.IsPaid,
                LastPaidDate = bill.LastPaidDate,
                CategoryId = bill.CategoryId,
                CategoryName = bill.Category?.Name ?? "Uncategorized"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting bill {billId}: {ex.Message}");
            throw;
        }
    }

    public async Task<BillDto> CreateBillAsync(int householdId, CreateBillDto request)
    {
        try
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Bill amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Bill name is required.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && 
                    (c.IsSystemDefault || c.HouseholdId == householdId));

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            var bill = new Bill
            {
                Name = request.Name.Trim(),
                Amount = request.Amount,
                DueDate = request.DueDate,
                Frequency = request.Frequency,
                IsPaid = false,
                CategoryId = request.CategoryId,
                HouseholdId = householdId
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Bill created: {request.Name} for household {householdId}");

            return new BillDto
            {
                Id = bill.Id,
                Name = bill.Name,
                Amount = bill.Amount,
                DueDate = bill.DueDate,
                Frequency = bill.Frequency,
                IsPaid = bill.IsPaid,
                LastPaidDate = bill.LastPaidDate,
                CategoryId = bill.CategoryId,
                CategoryName = category.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating bill: {ex.Message}");
            throw;
        }
    }

    public async Task<BillDto> UpdateBillAsync(int billId, int householdId, UpdateBillDto request)
    {
        try
        {
            var bill = await _context.Bills
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == billId && b.HouseholdId == householdId);

            if (bill == null)
                throw new KeyNotFoundException($"Bill {billId} not found.");

            if (request.Amount.HasValue && request.Amount <= 0)
                throw new ArgumentException("Bill amount must be greater than zero.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                bill.Name = request.Name.Trim();

            if (request.Amount.HasValue)
                bill.Amount = request.Amount.Value;

            if (request.DueDate.HasValue)
                bill.DueDate = request.DueDate.Value;

            if (!string.IsNullOrWhiteSpace(request.Frequency))
                bill.Frequency = request.Frequency;

            if (request.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId && 
                        (c.IsSystemDefault || c.HouseholdId == householdId));

                if (category == null)
                    throw new KeyNotFoundException("Category not found.");

                bill.CategoryId = request.CategoryId.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Bill updated: {billId}");

            return new BillDto
            {
                Id = bill.Id,
                Name = bill.Name,
                Amount = bill.Amount,
                DueDate = bill.DueDate,
                Frequency = bill.Frequency,
                IsPaid = bill.IsPaid,
                LastPaidDate = bill.LastPaidDate,
                CategoryId = bill.CategoryId,
                CategoryName = bill.Category?.Name ?? "Uncategorized"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating bill {billId}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteBillAsync(int billId, int householdId)
    {
        try
        {
            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == billId && b.HouseholdId == householdId);

            if (bill == null)
                throw new KeyNotFoundException($"Bill {billId} not found.");

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Bill deleted: {billId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting bill {billId}: {ex.Message}");
            throw;
        }
    }

    public async Task<BillDto> MarkBillAsPaidAsync(int billId, int householdId, MarkBillPaidDto request)
    {
        try
        {
            var bill = await _context.Bills
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == billId && b.HouseholdId == householdId);

            if (bill == null)
                throw new KeyNotFoundException($"Bill {billId} not found.");

            bill.IsPaid = true;
            bill.LastPaidDate = request.PaidDate;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Bill marked as paid: {billId}");

            return new BillDto
            {
                Id = bill.Id,
                Name = bill.Name,
                Amount = bill.Amount,
                DueDate = bill.DueDate,
                Frequency = bill.Frequency,
                IsPaid = bill.IsPaid,
                LastPaidDate = bill.LastPaidDate,
                CategoryId = bill.CategoryId,
                CategoryName = bill.Category?.Name ?? "Uncategorized"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error marking bill as paid {billId}: {ex.Message}");
            throw;
        }
    }
}
