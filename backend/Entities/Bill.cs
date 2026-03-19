namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents a recurring or one-time bill.
/// </summary>
public class Bill : BaseEntity
{
    /// <summary>
    /// Name of the bill (e.g., "Electric Bill", "Internet").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Amount of the bill.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Due date of the bill.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Frequency of the bill: "OneTime", "Monthly", "Quarterly", "Annual".
    /// </summary>
    public string Frequency { get; set; } = "Monthly";

    /// <summary>
    /// Indicates if this bill has been paid.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Last date this bill was marked as paid (for recurring bills).
    /// </summary>
    public DateTime? LastPaidDate { get; set; }

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
