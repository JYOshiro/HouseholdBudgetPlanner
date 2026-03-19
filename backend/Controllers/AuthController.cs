using FluentValidation;
using HouseholdBudgetApi.DTOs.Auth;
using HouseholdBudgetApi.Helpers;
using HouseholdBudgetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdBudgetApi.Controllers;

/// <summary>
/// Authentication controller for user registration, login, and account management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user and creates a household.
    /// </summary>
    /// <param name="request">Registration request containing email, password, names, and household name</param>
    /// <returns>Authentication response with JWT token on success</returns>
    /// <response code="200">Registration successful, returns auth response with token</response>
    /// <response code="400">Validation failed or email already exists</response>
    /// <response code="500">Server error during registration</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            // Validate request
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(e => e.ErrorMessage).Distinct().ToArray()
                    );
                return BadRequest(new { message = "Validation failed", errors });
            }

            // Register user
            var response = await _authService.RegisterAsync(request);
            _logger.LogInformation($"User registered successfully: {request.Email}");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Registration failed: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error during registration: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An unexpected error occurred during registration" });
        }
    }

    /// <summary>
    /// Authenticates user with email and password.
    /// </summary>
    /// <param name="request">Login request containing email and password</param>
    /// <returns>Authentication response with JWT token on success</returns>
    /// <response code="200">Login successful, returns auth response with token</response>
    /// <response code="400">Validation failed</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="500">Server error during login</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Validate request
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(e => e.ErrorMessage).Distinct().ToArray()
                    );
                return BadRequest(new { message = "Validation failed", errors });
            }

            // Authenticate user
            var response = await _authService.LoginAsync(request);
            _logger.LogInformation($"User logged in successfully: {request.Email}");

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning($"Login failed for email {request.Email}: Invalid credentials");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error during login: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An unexpected error occurred during login" });
        }
    }

    /// <summary>
    /// Gets current authenticated user information.
    /// </summary>
    /// <returns>Current user DTO on success</returns>
    /// <response code="200">Returns current user information</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Server error retrieving user</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
    {
        try
        {
            // Extract user ID from claims
            var userId = User.GetUserId();
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid user ID in claims");
                return Unauthorized(new { message = "Invalid user context" });
            }

            // Get user info
            var userDto = await _authService.GetCurrentUserAsync(userId);
            if (userDto == null)
            {
                _logger.LogWarning($"User not found: {userId}");
                return NotFound(new { message = "User not found" });
            }

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error retrieving current user: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An unexpected error occurred" });
        }
    }
}
