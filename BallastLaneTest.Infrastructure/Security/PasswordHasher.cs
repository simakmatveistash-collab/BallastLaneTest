using BallastLaneTest.Application.Services;

namespace BallastLaneTest.Infrastructure.Security;

/// <summary>
/// Password hasher implementation using PBKDF2
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8; // 128 bits
    private const int KeySize = 256 / 8; // 256 bits
    private const int Iterations = 10000;

    public string HashPassword(string password)
    {
        using (var algorithm = new System.Security.Cryptography.Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            System.Security.Cryptography.HashAlgorithmName.SHA256))
        {
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{Iterations}.{salt}.{key}";
        }
    }

    public bool VerifyPassword(string password, string hash)
    {
        var parts = hash.Split('.', 3);
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        using (var algorithm = new System.Security.Cryptography.Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            System.Security.Cryptography.HashAlgorithmName.SHA256))
        {
            var keyToCheck = algorithm.GetBytes(KeySize);
            return keyToCheck.SequenceEqual(key);
        }
    }
}
