using HouseholdBudgetApi.DTOs.Category;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Category service interface for managing expense/income categories.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Gets all categories for a household (including system defaults).
    /// </summary>
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int householdId, string? type = null);

    /// <summary>
    /// Gets a single category by ID.
    /// </summary>
    Task<CategoryDto?> GetCategoryAsync(int categoryId, int householdId);

    /// <summary>
    /// Creates a new custom category for a household.
    /// </summary>
    Task<CategoryDto> CreateCategoryAsync(int householdId, CreateCategoryDto request);

    /// <summary>
    /// Updates a category (only custom categories can be updated).
    /// </summary>
    Task<CategoryDto> UpdateCategoryAsync(int categoryId, int householdId, UpdateCategoryDto request);

    /// <summary>
    /// Deletes a category (only custom categories can be deleted).
    /// </summary>
    Task DeleteCategoryAsync(int categoryId, int householdId);
}
