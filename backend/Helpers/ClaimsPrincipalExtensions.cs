using System.Security.Claims;

namespace HouseholdBudgetApi.Helpers;

/// <summary>
/// Extension methods for extracting claims from ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the claims principal.
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Gets the user email from the claims principal.
    /// </summary>
    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the household ID from the claims principal.
    /// </summary>
    public static int GetHouseholdId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst("HouseholdId");
        return int.TryParse(claim?.Value, out var householdId) ? householdId : 0;
    }
}
