namespace BallastLaneTest.Application.Services;

/// <summary>
/// Service interface for password hashing and verification
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a plaintext password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify a plaintext password against a hash
    /// </summary>
    bool VerifyPassword(string password, string hash);
}
