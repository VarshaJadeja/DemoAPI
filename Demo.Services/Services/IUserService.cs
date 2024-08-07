using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using FluentResults;

namespace Demo.Services.Services;

public interface IUserService
{
    public Task<User> GetUserByEmailAsync(string email);
    public Task UpdatePasswordAsync(string email, string newPassword);
    //public Task UpdateFieldAsync(string email, string fieldName, string newValue);
    public Task UpdateFieldAsync(string email, Dictionary<string, object> fieldUpdates);
    public Task<Result<LoginResponse>> Login(LoginRequest loginRequest);
    public Task<Result<User>> Register(RegistrationRequest user);
}