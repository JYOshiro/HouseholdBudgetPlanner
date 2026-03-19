using HouseholdBudgetApi.Data;
using HouseholdBudgetApi.DTOs.Auth;
using HouseholdBudgetApi.Entities;
using HouseholdBudgetApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HouseholdBudgetApi.Services;

/// <summary>
/// Authentication service implementation for user registration, login, and retrieval.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IJwtTokenGenerator tokenGenerator,
        ILogger<AuthService> logger)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user and creates their household.
    /// </summary>
    /// <param name="request">Registration request with email, password, name, and household name</param>
    /// <returns>Authentication response with JWT token and user info</returns>
    /// <exception cref="InvalidOperationException">Thrown if email already exists</exception>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // Normalize email to lowercase for consistency
            var emailNormalized = request.Email.ToLower().Trim();

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == emailNormalized);

            if (existingUser != null)
            {
                _logger.LogWarning($"Registration attempt with existing email: {request.Email}");
                throw new InvalidOperationException("A user with this email already exists.");
            }

            // Create new household
            var household = new Household
            {
                Name = request.HouseholdName,
                CurrencySymbol = "$"
            };

            // Create new user
            var user = new User
            {
                Email = emailNormalized,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                HouseholdId = 0 // Will be set by EF after household is saved
            };

            household.Users.Add(user);
            _context.Households.Add(household);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User registered successfully: {user.Email} with household: {household.Name}");

            // Generate JWT token
            var token = _tokenGenerator.GenerateToken(user.Id, user.Email, household.Id);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresIn = 3600, // 1 hour in seconds
                User = new CurrentUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    HouseholdId = household.Id
                }
            };
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError($"Database error during registration: {ex.Message}");
            throw new InvalidOperationException("An error occurred during registration. Please try again.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during registration: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Authenticates a user by email and password.
    /// </summary>
    /// <param name="request">Login request with email and password</param>
    /// <returns>Authentication response with JWT token and user info</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if credentials are invalid</exception>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var emailNormalized = request.Email.ToLower().Trim();

            // Find user by email
            var user = await _context.Users
                .Include(u => u.Household)
                .FirstOrDefaultAsync(u => u.Email == emailNormalized);

            if (user == null)
            {
                _logger.LogWarning($"Login attempt with non-existent email: {request.Email}");
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Verify password
            if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"Failed login attempt for user: {user.Email}");
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            _logger.LogInformation($"User logged in successfully: {user.Email}");

            // Generate JWT token
            var token = _tokenGenerator.GenerateToken(user.Id, user.Email, user.HouseholdId);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresIn = 3600, // 1 hour in seconds
                User = new CurrentUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    HouseholdId = user.HouseholdId
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during login: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the current user's information based on their ID.
    /// </summary>
    /// <param name="userId">The user ID to retrieve</param>
    /// <returns>Current user DTO with user info, or null if user not found</returns>
    public async Task<CurrentUserDto?> GetCurrentUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning($"GetCurrentUser called for non-existent user: {userId}");
                return null;
            }

            return new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                HouseholdId = user.HouseholdId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving current user: {ex.Message}");
            throw;
        }
    }
}
