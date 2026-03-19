using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Household;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Household service implementation.
/// </summary>
public class HouseholdService : IHouseholdService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HouseholdService> _logger;

    public HouseholdService(ApplicationDbContext context, ILogger<HouseholdService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HouseholdDto?> GetHouseholdAsync(int householdId)
    {
        try
        {
            var household = await _context.Households
                .Include(h => h.Users)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == householdId);

            if (household == null)
                return null;

            return new HouseholdDto
            {
                Id = household.Id,
                Name = household.Name,
                CurrencySymbol = household.CurrencySymbol,
                Members = household.Users.Select(u => new HouseholdMemberDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting household {householdId}: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<HouseholdMemberDto>> GetHouseholdMembersAsync(int householdId)
    {
        try
        {
            var members = await _context.Users
                .Where(u => u.HouseholdId == householdId)
                .OrderBy(u => u.FirstName)
                .AsNoTracking()
                .ToListAsync();

            return members.Select(u => new HouseholdMemberDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting household members for household {householdId}: {ex.Message}");
            throw;
        }
    }
}
