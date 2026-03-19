namespace HouseholdBudgetApi.Helpers;

/// <summary>
/// Utility class for hashing and verifying passwords using bcrypt.
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// Hashes a password using bcrypt with a default work factor of 12.
    /// </summary>
    /// <param name="password">Plain text password to hash.</param>
    /// <returns>Hashed password.</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// Verifies a plain text password against a stored hash.
    /// </summary>
    /// <param name="password">Plain text password to verify.</param>
    /// <param name="hash">Previously hashed password.</param>
    /// <returns>True if password matches the hash; otherwise false.</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
