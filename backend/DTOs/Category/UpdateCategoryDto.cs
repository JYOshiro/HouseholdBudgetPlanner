namespace HouseholdBudgetApi.DTOs.Category;

/// <summary>
/// DTO for updating a category.
/// </summary>
public class UpdateCategoryDto
{
    /// <summary>
    /// Category name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Color code for UI display.
    /// </summary>
    public string? Color { get; set; }
}
