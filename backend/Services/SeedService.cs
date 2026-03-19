using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Service for seeding default data into the database.
/// </summary>
public interface ISeedService
{
    /// <summary>
    /// Seeds default expense and income categories if they don't already exist.
    /// </summary>
    Task SeedDefaultCategoriesAsync();
}

public class SeedService : ISeedService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SeedService> _logger;

    public SeedService(ApplicationDbContext context, ILogger<SeedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds default expense and income categories if they don't already exist.
    /// </summary>
    public async Task SeedDefaultCategoriesAsync()
    {
        try
        {
            // Check if default categories already exist
            var existingCount = await _context.Categories
                .Where(c => c.IsSystemDefault)
                .CountAsync();

            if (existingCount > 0)
            {
                _logger.LogInformation("Default categories already exist. Skipping seed.");
                return;
            }

            var defaultCategories = new List<Category>
            {
                // Expense Categories
                new() { Name = "Groceries", Type = "Expense", IsSystemDefault = true, Color = "#FF6B6B" },
                new() { Name = "Transportation", Type = "Expense", IsSystemDefault = true, Color = "#4ECDC4" },
                new() { Name = "Utilities", Type = "Expense", IsSystemDefault = true, Color = "#45B7D1" },
                new() { Name = "Entertainment", Type = "Expense", IsSystemDefault = true, Color = "#FFA07A" },
                new() { Name = "Healthcare", Type = "Expense", IsSystemDefault = true, Color = "#98D8C8" },
                new() { Name = "Housing", Type = "Expense", IsSystemDefault = true, Color = "#6C5CE7" },
                new() { Name = "Dining Out", Type = "Expense", IsSystemDefault = true, Color = "#A29BFE" },
                new() { Name = "Shopping", Type = "Expense", IsSystemDefault = true, Color = "#FD79A8" },
                new() { Name = "Insurance", Type = "Expense", IsSystemDefault = true, Color = "#55EFC4" },
                new() { Name = "Subscriptions", Type = "Expense", IsSystemDefault = true, Color = "#74B9FF" },
                new() { Name = "Education", Type = "Expense", IsSystemDefault = true, Color = "#DFE6E9" },
                new() { Name = "Personal Care", Type = "Expense", IsSystemDefault = true, Color = "#FF7675" },
                new() { Name = "Other Expense", Type = "Expense", IsSystemDefault = true, Color = "#B2BEB5" },

                // Income Categories
                new() { Name = "Salary", Type = "Income", IsSystemDefault = true, Color = "#00B894" },
                new() { Name = "Freelance", Type = "Income", IsSystemDefault = true, Color = "#6C5CE7" },
                new() { Name = "Investment", Type = "Income", IsSystemDefault = true, Color = "#FDCB6E" },
                new() { Name = "Bonus", Type = "Income", IsSystemDefault = true, Color = "#55EFC4" },
                new() { Name = "Reimbursement", Type = "Income", IsSystemDefault = true, Color = "#74B9FF" },
                new() { Name = "Gift", Type = "Income", IsSystemDefault = true, Color = "#FD79A8" },
                new() { Name = "Other Income", Type = "Income", IsSystemDefault = true, Color = "#B2BEB5" }
            };

            await _context.Categories.AddRangeAsync(defaultCategories);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully seeded {defaultCategories.Count} default categories.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error seeding default categories: {ex.Message}");
            throw;
        }
    }
}
