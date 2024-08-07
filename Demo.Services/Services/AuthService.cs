using System.Security.Cryptography;

namespace Demo.Services.Services;

public class AuthService : IAuthService
{
    private const int TokenExpirationMinutes = 60;

    public string GenerateToken()
    {
        byte[] tokenBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }

        string token = Convert.ToBase64String(tokenBytes);
        return token;
    }
    public DateTime CalculateExpirationTime()
    {
        return DateTime.UtcNow.AddMinutes(TokenExpirationMinutes);
    }
}
