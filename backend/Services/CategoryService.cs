using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Category;
using HouseholdBudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Category service implementation for managing expense and income categories.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets all categories for a household, including system defaults and custom categories.
    /// </summary>
    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int householdId, string? type = null)
    {
        try
        {
            var query = _context.Categories
                .Where(c => c.IsSystemDefault || c.HouseholdId == householdId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(c => c.Type == type);
            }

            var categories = await query
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                IsSystemDefault = c.IsSystemDefault,
                Color = c.Color
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting categories for household {householdId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets a single category by ID, verifying it belongs to the household.
    /// </summary>
    public async Task<CategoryDto?> GetCategoryAsync(int categoryId, int householdId)
    {
        try
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => 
                    c.Id == categoryId && 
                    (c.IsSystemDefault || c.HouseholdId == householdId));

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                IsSystemDefault = category.IsSystemDefault,
                Color = category.Color
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting category {categoryId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Creates a new custom category for a household.
    /// </summary>
    public async Task<CategoryDto> CreateCategoryAsync(int householdId, CreateCategoryDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Category name is required.");

            if (string.IsNullOrWhiteSpace(request.Type) || (request.Type != "Expense" && request.Type != "Income"))
                throw new ArgumentException("Type must be either 'Expense' or 'Income'.");

            var category = new Category
            {
                Name = request.Name.Trim(),
                Type = request.Type,
                Color = request.Color ?? "#808080",
                IsSystemDefault = false,
                HouseholdId = householdId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Category created: {category.Name} for household {householdId}");

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                IsSystemDefault = category.IsSystemDefault,
                Color = category.Color
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating category: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates a category (only custom categories can be updated).
    /// </summary>
    public async Task<CategoryDto> UpdateCategoryAsync(int categoryId, int householdId, UpdateCategoryDto request)
    {
        try
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.HouseholdId == householdId);

            if (category == null)
                throw new KeyNotFoundException($"Category {categoryId} not found.");

            if (category.IsSystemDefault)
                throw new InvalidOperationException("System default categories cannot be modified.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                category.Name = request.Name.Trim();

            if (!string.IsNullOrWhiteSpace(request.Color))
                category.Color = request.Color;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Category updated: {category.Name}");

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                IsSystemDefault = category.IsSystemDefault,
                Color = category.Color
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating category {categoryId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes a custom category (only custom categories can be deleted).
    /// </summary>
    public async Task DeleteCategoryAsync(int categoryId, int householdId)
    {
        try
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.HouseholdId == householdId);

            if (category == null)
                throw new KeyNotFoundException($"Category {categoryId} not found.");

            if (category.IsSystemDefault)
                throw new InvalidOperationException("System default categories cannot be deleted.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Category deleted: {category.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting category {categoryId}: {ex.Message}");
            throw;
        }
    }
}
