using HouseholdBudgetApi.DTOs.Category;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Categories controller for managing expense and income categories.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all categories for the user's household.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery] string? type = null)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetCategories called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var categories = await _categoryService.GetCategoriesAsync(householdId, type);
        return Ok(categories);
    }

    /// <summary>
    /// Gets a single category by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("GetCategory called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var category = await _categoryService.GetCategoryAsync(id, householdId);
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        return Ok(category);
    }

    /// <summary>
    /// Creates a new custom category.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("CreateCategory called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var category = await _categoryService.CreateCategoryAsync(householdId, request);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    /// <summary>
    /// Updates a category.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto request)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("UpdateCategory called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        var category = await _categoryService.UpdateCategoryAsync(id, householdId, request);
        return Ok(category);
    }

    /// <summary>
    /// Deletes a category.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId <= 0)
        {
            _logger.LogWarning("DeleteCategory called with invalid household claim.");
            return Unauthorized(new { message = "Invalid user context" });
        }

        await _categoryService.DeleteCategoryAsync(id, householdId);
        return NoContent();
    }
}
