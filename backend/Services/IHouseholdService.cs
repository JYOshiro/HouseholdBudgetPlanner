using HouseholdBudgetApi.DTOs.Household;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Household service interface for managing household data.
/// </summary>
public interface IHouseholdService
{
    /// <summary>
    /// Gets household details by ID.
    /// </summary>
    Task<HouseholdDto?> GetHouseholdAsync(int householdId);

    /// <summary>
    /// Gets all members of a household.
    /// </summary>
    Task<IEnumerable<HouseholdMemberDto>> GetHouseholdMembersAsync(int householdId);
}
