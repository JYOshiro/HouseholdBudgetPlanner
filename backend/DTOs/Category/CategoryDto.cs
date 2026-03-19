namespace HouseholdBudgetApi.DTOs.Category;

/// <summary>
/// DTO for category information.
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Category ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of category: "Expense" or "Income".
    /// </summary>
    public string Type { get; set; } = "Expense";

    /// <summary>
    /// Indicates if this is a system default category.
    /// </summary>
    public bool IsSystemDefault { get; set; }

    /// <summary>
    /// Color code for UI display.
    /// </summary>
    public string? Color { get; set; }
}
