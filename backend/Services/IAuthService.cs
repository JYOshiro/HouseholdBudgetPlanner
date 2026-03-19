using HouseholdBudgetApi.DTOs.Auth;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Authentication service interface for user registration and login.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and creates a household.
    /// </summary>
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// Authenticates user with email and password.
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Gets current user information by ID.
    /// </summary>
    Task<CurrentUserDto?> GetCurrentUserAsync(int userId);
}
