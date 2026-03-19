namespace HouseholdBudgetApi.Entities;

using Base;

/// <summary>
/// Represents a household containing multiple users and financial records.
/// </summary>
public class Household : BaseEntity
{
    /// <summary>
    /// Name of the household (e.g., "Smith Family").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Currency symbol used for this household (e.g., "$", "€", "£").
    /// </summary>
    public string CurrencySymbol { get; set; } = "$";

    /// <summary>
    /// Navigation property to all users in this household.
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Navigation property to all expenses recorded in this household.
    /// </summary>
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    /// <summary>
    /// Navigation property to all income entries recorded in this household.
    /// </summary>
    public virtual ICollection<Income> IncomeEntries { get; set; } = new List<Income>();

    /// <summary>
    /// Navigation property to all categories in this household.
    /// </summary>
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    /// <summary>
    /// Navigation property to all budgets in this household.
    /// </summary>
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    /// <summary>
    /// Navigation property to all bills in this household.
    /// </summary>
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    /// <summary>
    /// Navigation property to all savings goals in this household.
    /// </summary>
    public virtual ICollection<SavingsGoal> SavingsGoals { get; set; } = new List<SavingsGoal>();
}
