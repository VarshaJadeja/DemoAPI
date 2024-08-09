using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Demo.Repositories;
using Demo.Repositories.Errors;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demo.Services.Services;

public class TokenService : ITokenService
{
    private readonly IUserRepository UserRepository;
    private string secretKey;

    public TokenService(IUserRepository UserRepository, IConfiguration configuration)
    {
        this.UserRepository = UserRepository;
        secretKey = configuration.GetValue<string>("ApiSetting:Secret");
    }
 
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);
        var expire = DateTime.UtcNow.AddMinutes(1);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Expiration, expire.ToString()),
            }),
            Expires = expire,
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var refreshToken = Guid.NewGuid().ToString();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var writeToken = tokenHandler.WriteToken(token);
        return writeToken;
    }
    public async Task<Result<RefreshResponse>> RefreshToken(string refreshToken)
    {
        // Retrieve the user associated with the refresh token from the database
        var user = await UserRepository.GetUserByRefreshTokenAsync(refreshToken);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return Result.Fail(FluentError.UnAuthorized(ErrorType.UnAuthorized, "Token Expired"));
        }
        var newAccessToken = GenerateToken(user);
        var updates = new Dictionary<string, object>
         {
             { "AccessToken", newAccessToken }
         };
        await UserRepository.UpdateFieldsAsync(user.Email, updates);
        return new RefreshResponse()
        {
            Token = newAccessToken
        };
    }

}
