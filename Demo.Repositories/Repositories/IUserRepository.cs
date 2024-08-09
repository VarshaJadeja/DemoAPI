using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using FluentResults;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Demo.Repositories;

public interface IUserRepository
{
    public Task<User> GetUserByEmailAsync(string email);
    public Task<bool> UpdatePasswordAsync(string email, string newPassword);
    public Task<bool> UpdateFieldAsync(string email, string fieldName, object newValue);
    public Task<bool> UpdateFieldsAsync(string email, Dictionary<string, object> fieldUpdates);
    bool IsUniqueUser(string username);
    public Task<User> GetUserByRefreshTokenAsync(string refreshToken);
    Task<Result<User>> Register(RegistrationRequest registrationRequest);
    public Task<User> GetUser(LoginRequest loginRequest);
}
