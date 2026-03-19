namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents an expense category (e.g., Groceries, Utilities, Entertainment).
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Name of the category.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of category: "Expense" or "Income".
    /// </summary>
    public string Type { get; set; } = "Expense"; // "Expense" or "Income"

    /// <summary>
    /// Indicates if this is a system default category (cannot be deleted by users).
    /// </summary>
    public bool IsSystemDefault { get; set; }

    /// <summary>
    /// Optional color code for UI display (e.g., "#FF5733").
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Foreign key to the household. Null for system categories.
    /// </summary>
    public int? HouseholdId { get; set; }

    /// <summary>
    /// Navigation property to the household (null for system categories).
    /// </summary>
    public virtual Household? Household { get; set; }

    /// <summary>
    /// Navigation property to all expenses under this category.
    /// </summary>
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    /// <summary>
    /// Navigation property to all income entries under this category.
    /// </summary>
    public virtual ICollection<Income> IncomeEntries { get; set; } = new List<Income>();

    /// <summary>
    /// Navigation property to all budgets for this category.
    /// </summary>
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    /// <summary>
    /// Navigation property to all bills for this category.
    /// </summary>
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
