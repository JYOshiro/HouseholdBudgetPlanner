namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents an income entry in the household.
/// </summary>
public class Income : BaseEntity
{
    /// <summary>
    /// Amount of income.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Source of income (e.g., "Salary", "Bonus", "Freelance").
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Date of the income entry.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Foreign key to the household.
    /// </summary>
    public int HouseholdId { get; set; }

    /// <summary>
    /// Foreign key to the category.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Foreign key to the user who recorded this income.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation property to the household.
    /// </summary>
    public virtual Household? Household { get; set; }

    /// <summary>
    /// Navigation property to the category.
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public virtual User? User { get; set; }
}
