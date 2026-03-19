namespace HouseholdBudgetApi.DTOs.Household;

/// <summary>
/// DTO for household member information.
/// </summary>
public class HouseholdMemberDto
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
    /// Full name of the user.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}
