using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HouseholdBudgetApi.Config;
using Microsoft.IdentityModel.Tokens;

namespace HouseholdBudgetApi.Helpers;

/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the given user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="email">User email.</param>
    /// <param name="householdId">Household ID the user belongs to.</param>
    /// <returns>JWT token string.</returns>
    string GenerateToken(int userId, string email, int householdId);
}

/// <summary>
/// Default implementation of JWT token generation.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
    }

    /// <summary>
    /// Generates a JWT token with user claims.
    /// </summary>
    public string GenerateToken(int userId, string email, int householdId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("HouseholdId", householdId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
