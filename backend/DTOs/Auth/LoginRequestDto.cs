namespace HouseholdBudgetApi.DTOs.Auth;

/// <summary>
/// DTO for user login request.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
