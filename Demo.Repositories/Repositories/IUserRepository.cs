using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using FluentResults;
namespace Demo.Repositories;

public interface IUserRepository
{
    public Task<User> GetUserByEmailAsync(string email);
    public Task<bool> UpdatePasswordAsync(string email, string newPassword);
    public Task<bool> UpdateFieldAsync(string email, string fieldName, object newValue);
    public Task<bool> UpdateFieldsAsync(string email, Dictionary<string, object> fieldUpdates);
    bool IsUniqueUser(string username);
    Task<Result<LoginResponse>> Login(LoginRequest oginRequest);
    Task<Result<User>> Register(RegistrationRequest registrationRequest);
}
