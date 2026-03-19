namespace HouseholdBudgetApi.DTOs.Category;

/// <summary>
/// DTO for creating a new category.
/// </summary>
public class CreateCategoryDto
{
    /// <summary>
    /// Category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of category: "Expense" or "Income".
    /// </summary>
    public string Type { get; set; } = "Expense";

    /// <summary>
    /// Color code for UI display (optional).
    /// </summary>
    public string? Color { get; set; }
}
