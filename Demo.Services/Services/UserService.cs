using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Demo.Repositories;
using Demo.Repositories.Constants;
using Demo.Repositories.Errors;
using FluentResults;

namespace Demo.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository UserRepository;
    private readonly ITokenService TokenService;

    public UserService(IUserRepository UserRepository, ITokenService TokenService) 
    {
        this.UserRepository = UserRepository;
        this.TokenService = TokenService;
    }

    public async Task<Result<LoginResponse>> Login(LoginRequest loginRequest)
    {
        User user = await UserRepository.GetUser(loginRequest);
        if (user == null)
        {
            return Result.Fail(FluentError.InvalidCredentials(ErrorType.InvalidCredentials, ErrorMessages.InvalidCredentials));
        }
        var refreshToken = Guid.NewGuid().ToString();
        var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(3);
        var token = TokenService.GenerateToken(user);
        var updates = new Dictionary<string, object>
        {
            { "AccessToken", token },
            { "RefreshToken", refreshToken },
            { "RefreshTokenExpiry", refreshTokenExpiry }
        };
        await UserRepository.UpdateFieldsAsync(user.Email, updates);
        var updatedUser = await UserRepository.GetUser(loginRequest);
        LoginResponse loginResponse = new LoginResponse()
        {
            Token = token,
            RefreshToken = refreshToken,
            User = updatedUser,
        };

        return loginResponse;
    }

    public async Task<Result<User>> Register(RegistrationRequest user)
    {
        var response = await UserRepository.Register(user);
        return response;
    }
    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await UserRepository.GetUserByEmailAsync(email);
        return user;
    }

    public async Task UpdateFieldAsync(string email, Dictionary<string, object> fieldUpdates)
    {
        await UserRepository.UpdateFieldsAsync(email, fieldUpdates);
    }
   
}
