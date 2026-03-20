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

            if (request.TargetDate.HasValue)
                goal.TargetDate = request.TargetDate.Value;
            else if (request.TargetDate == null && request.GetType().GetProperty(nameof(request.TargetDate)) != null)
                goal.TargetDate = null;

            if (request.TargetAmount.HasValue)
                goal.TargetAmount = request.TargetAmount.Value;

            if (!string.IsNullOrWhiteSpace(request.Priority))
                goal.Priority = NormalizePriority(request.Priority);

            // Handle explicit status change (archive or reopen).
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var normalizedStatus = NormalizeStatus(request.Status);
                goal.Status = normalizedStatus;
                if (normalizedStatus == "Active")
                    goal.CompletedDate = null;
                else if (normalizedStatus == "Completed" && goal.CompletedDate == null)
                    goal.CompletedDate = DateTime.UtcNow;
            }

            // Auto-recalculate completion when target amount changes.
            if (request.TargetAmount.HasValue && goal.Status != "Archived")
            {
                if (goal.CurrentAmount >= goal.TargetAmount && goal.Status != "Completed")
                {
                    goal.Status = "Completed";
                    goal.CompletedDate ??= DateTime.UtcNow;
                }
                else if (goal.CurrentAmount < goal.TargetAmount && goal.Status == "Completed")
                {
                    goal.Status = "Active";
                    goal.CompletedDate = null;
                }
            }

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
            Status = goal.Status,
            CompletedDate = goal.CompletedDate,
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

    private static string NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return "Active";

        return status.Trim().ToLowerInvariant() switch
        {
            "active" => "Active",
            "completed" => "Completed",
            "archived" => "Archived",
            _ => throw new ArgumentException("Status must be one of: Active, Completed, Archived."),
        };
    }
}
