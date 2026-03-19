namespace HouseholdBudgetApi.DTOs.Auth;

/// <summary>
/// DTO for user registration request.
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password (will be hashed server-side).
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the household to create.
    /// </summary>
    public string HouseholdName { get; set; } = string.Empty;
}
