namespace HouseholdBudgetApi.DTOs.Household;

/// <summary>
/// DTO for creating a new household.
/// </summary>
public class CreateHouseholdDto
{
    /// <summary>
    /// Household name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Currency symbol for this household.
    /// </summary>
    public string CurrencySymbol { get; set; } = "$";
}
