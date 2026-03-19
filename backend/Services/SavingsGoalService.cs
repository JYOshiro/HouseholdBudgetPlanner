using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.SavingsGoal;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Savings Goal service implementation.
/// </summary>
public class SavingsGoalService : ISavingsGoalService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SavingsGoalService> _logger;

    public SavingsGoalService(ApplicationDbContext context, ILogger<SavingsGoalService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SavingsGoalDto>> GetGoalsAsync(int householdId)
    {
        try
        {
            var goals = await _context.SavingsGoals
                .Where(g => g.HouseholdId == householdId)
                .OrderBy(g => g.TargetDate ?? DateTime.MaxValue)
                .ThenBy(g => g.Name)
                .AsNoTracking()
                .ToListAsync();

            return goals.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting savings goals for household {householdId}: {ex.Message}");
            throw;
        }
    }

    public async Task<SavingsGoalDto?> GetGoalAsync(int goalId, int householdId)
    {
        try
        {
            var goal = await _context.SavingsGoals
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == goalId && g.HouseholdId == householdId);

            return goal == null ? null : MapToDto(goal);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting savings goal {goalId}: {ex.Message}");
            throw;
        }
    }

    public async Task<SavingsGoalDto> CreateGoalAsync(int householdId, CreateSavingsGoalDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Savings goal name is required.");

            if (request.TargetAmount <= 0)
                throw new ArgumentException("Target amount must be greater than zero.");

            var goal = new SavingsGoal
            {
                Name = request.Name.Trim(),
                TargetAmount = request.TargetAmount,
                CurrentAmount = 0,
                TargetDate = request.TargetDate,
                Priority = NormalizePriority(request.Priority),
                HouseholdId = householdId,
            };

            _context.SavingsGoals.Add(goal);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Savings goal created: {goal.Name} for household {householdId}");

            return MapToDto(goal);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating savings goal: {ex.Message}");
            throw;
        }
    }

    public async Task<SavingsGoalDto> UpdateGoalAsync(int goalId, int householdId, UpdateSavingsGoalDto request)
    {
        try
        {
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.HouseholdId == householdId);

            if (goal == null)
                throw new KeyNotFoundException($"Savings goal {goalId} not found.");

            if (request.TargetAmount.HasValue && request.TargetAmount <= 0)
                throw new ArgumentException("Target amount must be greater than zero.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                goal.Name = request.Name.Trim();

            if (request.TargetAmount.HasValue)
                goal.TargetAmount = request.TargetAmount.Value;

            if (request.TargetDate.HasValue)
                goal.TargetDate = request.TargetDate.Value;

            if (!string.IsNullOrWhiteSpace(request.Priority))
                goal.Priority = NormalizePriority(request.Priority);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Savings goal updated: {goalId}");

            return MapToDto(goal);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating savings goal {goalId}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteGoalAsync(int goalId, int householdId)
    {
        try
        {
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.HouseholdId == householdId);

            if (goal == null)
                throw new KeyNotFoundException($"Savings goal {goalId} not found.");

            _context.SavingsGoals.Remove(goal);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Savings goal deleted: {goalId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting savings goal {goalId}: {ex.Message}");
            throw;
        }
    }

    private static SavingsGoalDto MapToDto(SavingsGoal goal)
    {
        return new SavingsGoalDto
        {
            Id = goal.Id,
            Name = goal.Name,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            TargetDate = goal.TargetDate,
            Priority = goal.Priority,
        };
    }

    private static string NormalizePriority(string? priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
            return "Normal";

        return priority.Trim().ToLowerInvariant() switch
        {
            "high" => "High",
            "normal" => "Normal",
            "low" => "Low",
            _ => throw new ArgumentException("Priority must be one of: High, Normal, Low."),
        };
    }
}
