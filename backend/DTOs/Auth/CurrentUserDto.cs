namespace HouseholdBudgetApi.DTOs.Auth;

/// <summary>
/// DTO for current authenticated user information.
/// </summary>
public class CurrentUserDto
{
    /// <summary>
    /// User ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Household ID.
    /// </summary>
    public int HouseholdId { get; set; }
}
