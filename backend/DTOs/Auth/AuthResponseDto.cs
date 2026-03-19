namespace HouseholdBudgetApi.DTOs.Auth;

/// <summary>
/// DTO for authentication response (login/register success).
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT access token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Current authenticated user information.
    /// </summary>
    public CurrentUserDto? User { get; set; }

    /// <summary>
    /// Token expiration time in seconds.
    /// </summary>
    public int ExpiresIn { get; set; }
}
