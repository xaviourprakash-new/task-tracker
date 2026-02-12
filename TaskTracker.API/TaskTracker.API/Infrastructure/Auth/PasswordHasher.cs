using System.Security.Cryptography;
using System.Text;
using TaskTracker.API.Application.Common.Interfaces;

namespace TaskTracker.API.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return $"{salt}:{hash}";
    }

    public bool Verify(string password, string hashedPassword)
    {
        try
        {
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2) return false;

            var key = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            using var hmac = new HMACSHA512(key);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
