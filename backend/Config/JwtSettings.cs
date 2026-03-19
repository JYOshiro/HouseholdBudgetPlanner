namespace HouseholdBudgetApi.Config;

/// <summary>
/// JWT authentication settings from configuration.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key for signing JWT tokens (minimum 32 characters for HS256).
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer claim.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience claim.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;
}
