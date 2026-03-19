namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents an expense entry in the household.
/// </summary>
public class Expense : BaseEntity
{
    /// <summary>
    /// Amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Description of the expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this expense is shared among household members.
    /// </summary>
    public bool IsShared { get; set; }

    /// <summary>
    /// Date of the expense.
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
    /// Foreign key to the user who recorded/paid this expense.
    /// </summary>
    public int PaidByUserId { get; set; }

    /// <summary>
    /// Navigation property to the household.
    /// </summary>
    public virtual Household? Household { get; set; }

    /// <summary>
    /// Navigation property to the category.
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Navigation property to the user who paid.
    /// </summary>
    public virtual User? PaidByUser { get; set; }
}
