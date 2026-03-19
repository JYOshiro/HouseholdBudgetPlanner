namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents a monthly budget for a specific expense category.
/// </summary>
public class Budget : BaseEntity
{
    /// <summary>
    /// Budgeted amount for the month.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Month for this budget (stored as DateTime with day set to 1st of the month).
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Foreign key to the household.
    /// </summary>
    public int HouseholdId { get; set; }

    /// <summary>
    /// Foreign key to the category.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Navigation property to the household.
    /// </summary>
    public virtual Household? Household { get; set; }

    /// <summary>
    /// Navigation property to the category.
    /// </summary>
    public virtual Category? Category { get; set; }
}
