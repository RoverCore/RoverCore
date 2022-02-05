using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace RoverCore.Boilerplate.Infrastructure.Extensions;

public class PasswordHash
{
    public string Salt { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}

public static class PasswordHasher
{
    public static PasswordHash Hash(this string password)
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return Hash(password, salt);
    }

    public static PasswordHash Hash(this string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        return Hash(password, saltBytes);
    }

    public static PasswordHash Hash(this string password, byte[] salt)
    {
        // derive a 256-bit subkey (use HMACSHA256 with 10,000 iterations)
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            10000,
            256 / 8));

        return new PasswordHash
        {
            Salt = Convert.ToBase64String(salt),
            HashedPassword = hashed
        };
    }
}