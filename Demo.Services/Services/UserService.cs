using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Demo.Repositories;
using FluentResults;

namespace Demo.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository UserRepository;

    public UserService(IUserRepository UserRepository) 
    {
        this.UserRepository = UserRepository;
    }

    public async Task<Result<LoginResponse>> Login(LoginRequest loginRequest)
    {
        var response = await UserRepository.Login(loginRequest);
        return response;
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

    public async Task UpdatePasswordAsync(string email, string newPassword)
    {
        await UserRepository.UpdatePasswordAsync(email, newPassword);
    }
    public async Task UpdateFieldAsync(string email, Dictionary<string, object> fieldUpdates)
    {
        await UserRepository.UpdateFieldsAsync(email, fieldUpdates);
    }
   
}
