using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.GoalContribution;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Goal Contribution service implementation.
/// </summary>
public class GoalContributionService : IGoalContributionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GoalContributionService> _logger;

    public GoalContributionService(ApplicationDbContext context, ILogger<GoalContributionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GoalContributionDto>> GetContributionsAsync(int goalId, int householdId)
    {
        try
        {
            var goal = await _context.SavingsGoals.FirstOrDefaultAsync(g => g.Id == goalId && g.HouseholdId == householdId);
            if (goal == null) throw new KeyNotFoundException($"Goal {goalId} not found.");

            var contributions = await _context.GoalContributions
                .Where(c => c.GoalId == goalId)
                .Include(c => c.ContributedByUser)
                .OrderByDescending(c => c.ContributionDate)
                .AsNoTracking()
                .ToListAsync();

            return contributions.Select(c => new GoalContributionDto
            {
                Id = c.Id,
                Amount = c.Amount,
                ContributionDate = c.ContributionDate,
                GoalId = c.GoalId,
                ContributedByUserId = c.ContributedByUserId,
                ContributedByUserName = c.ContributedByUser?.FirstName + " " + c.ContributedByUser?.LastName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting contributions for goal {goalId}: {ex.Message}");
            throw;
        }
    }

    public async Task<GoalContributionDto?> GetContributionAsync(int contributionId, int householdId)
    {
        try
        {
            var contribution = await _context.GoalContributions
                .Where(c => c.Goal.HouseholdId == householdId)
                .Include(c => c.Goal)
                .Include(c => c.ContributedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == contributionId);

            if (contribution == null) return null;

            return new GoalContributionDto
            {
                Id = contribution.Id,
                Amount = contribution.Amount,
                ContributionDate = contribution.ContributionDate,
                GoalId = contribution.GoalId,
                ContributedByUserId = contribution.ContributedByUserId,
                ContributedByUserName = contribution.ContributedByUser?.FirstName + " " + contribution.ContributedByUser?.LastName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting contribution {contributionId}: {ex.Message}");
            throw;
        }
    }

    public async Task<GoalContributionDto> CreateContributionAsync(int householdId, int userId, CreateGoalContributionDto request)
    {
        try
        {
            if (request.Amount <= 0) throw new ArgumentException("Contribution amount must be greater than zero.");

            var goal = await _context.SavingsGoals.FirstOrDefaultAsync(g => g.Id == request.GoalId && g.HouseholdId == householdId);
            if (goal == null) throw new KeyNotFoundException("Goal not found.");

            var contribution = new GoalContribution
            {
                Amount = request.Amount,
                ContributionDate = request.ContributionDate,
                GoalId = request.GoalId,
                ContributedByUserId = userId
            };

            goal.CurrentAmount += request.Amount;
            _context.GoalContributions.Add(contribution);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Contribution created for goal {request.GoalId}: {request.Amount}");

            return new GoalContributionDto
            {
                Id = contribution.Id,
                Amount = contribution.Amount,
                ContributionDate = contribution.ContributionDate,
                GoalId = contribution.GoalId,
                ContributedByUserId = contribution.ContributedByUserId,
                ContributedByUserName = ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating contribution: {ex.Message}");
            throw;
        }
    }

    public async Task<GoalContributionDto> UpdateContributionAsync(int contributionId, int householdId, UpdateGoalContributionDto request)
    {
        try
        {
            var contribution = await _context.GoalContributions
                .Include(c => c.Goal)
                .Include(c => c.ContributedByUser)
                .FirstOrDefaultAsync(c => c.Id == contributionId && c.Goal.HouseholdId == householdId);

            if (contribution == null) throw new KeyNotFoundException($"Contribution {contributionId} not found.");
            if (request.Amount.HasValue && request.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

            if (request.Amount.HasValue)
            {
                var difference = request.Amount.Value - contribution.Amount;
                contribution.Goal.CurrentAmount += difference;
                contribution.Amount = request.Amount.Value;
            }

            if (request.ContributionDate.HasValue)
                contribution.ContributionDate = request.ContributionDate.Value;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Contribution updated: {contributionId}");

            return new GoalContributionDto
            {
                Id = contribution.Id,
                Amount = contribution.Amount,
                ContributionDate = contribution.ContributionDate,
                GoalId = contribution.GoalId,
                ContributedByUserId = contribution.ContributedByUserId,
                ContributedByUserName = contribution.ContributedByUser?.FirstName + " " + contribution.ContributedByUser?.LastName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating contribution {contributionId}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteContributionAsync(int contributionId, int householdId)
    {
        try
        {
            var contribution = await _context.GoalContributions
                .Include(c => c.Goal)
                .FirstOrDefaultAsync(c => c.Id == contributionId && c.Goal.HouseholdId == householdId);

            if (contribution == null) throw new KeyNotFoundException($"Contribution {contributionId} not found.");

            contribution.Goal.CurrentAmount -= contribution.Amount;
            _context.GoalContributions.Remove(contribution);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Contribution deleted: {contributionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting contribution {contributionId}: {ex.Message}");
            throw;
        }
    }
}
