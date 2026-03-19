namespace HouseholdBudgetApi.DTOs.Household;

/// <summary>
/// DTO for household information.
/// </summary>
public class HouseholdDto
{
    /// <summary>
    /// Household ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Household name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Currency symbol used by this household.
    /// </summary>
    public string CurrencySymbol { get; set; } = "$";

    /// <summary>
    /// Household members.
    /// </summary>
    public ICollection<HouseholdMemberDto> Members { get; set; } = new List<HouseholdMemberDto>();
}
