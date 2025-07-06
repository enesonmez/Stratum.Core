using System.Security.Cryptography;
using System.Text;

namespace Core.Security.Hashing;

public class HashingHelper
{
    /// <summary>
    /// Create a password hash and salt via Rfc2898DeriveBytes (PBKDF2).
    /// </summary>
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var rng = RandomNumberGenerator.Create();
        passwordSalt = new byte[16];
        rng.GetBytes(passwordSalt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, passwordSalt, 100_000, HashAlgorithmName.SHA512);
        passwordHash = pbkdf2.GetBytes(32);
    }

    /// <summary>
    /// Verify a password hash and salt via Rfc2898DeriveBytes (PBKDF2).
    /// </summary>
    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, passwordSalt, 100_000, HashAlgorithmName.SHA512);
        byte[] computedHash = pbkdf2.GetBytes(32);
        return computedHash.SequenceEqual(passwordHash);
    }
}