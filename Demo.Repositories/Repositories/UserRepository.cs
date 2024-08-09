using Demo.Entities;
using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Demo.Repositories.Constants;
using Demo.Repositories.Errors;
using Microsoft.EntityFrameworkCore;

namespace Demo.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> collection;
    private string secretKey;
    public UserRepository(CustomsDeclarationsContext context, IConfiguration configuration, string collectionName = null)
    {
        if (string.IsNullOrEmpty(collectionName))
        {
            collectionName = typeof(User).Name;
        }
        collection = context.GetCollection<User>(collectionName);

        secretKey = configuration.GetValue<string>("ApiSetting:Secret");
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq("UserName", email);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UserName, email);
        var update = Builders<User>.Update.Set(u => u.Password, newPassword);

        var result = await collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
    public async Task<bool> UpdateFieldAsync(string email, string fieldName, object newValue)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UserName, email);
        var update = Builders<User>.Update.Set(fieldName, newValue);

        var result = await collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateFieldsAsync(string email, Dictionary<string, object> fieldUpdates)
    {
        var filter = Builders<User>.Filter.Eq("UserName", email);

        var updateDefinitions = fieldUpdates.Select(fieldUpdate =>
             Builders<User>.Update.Set(fieldUpdate.Key, fieldUpdate.Value)
         );
        var update = Builders<User>.Update.Combine(updateDefinitions);

        var result = await collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<User> GetUser(LoginRequest loginRequest)
    {
        var filter = Builders<User>.Filter.And(
                      Builders<User>.Filter.Eq("UserName", loginRequest.UserName),
                      Builders<User>.Filter.Eq("Password", loginRequest.Password)
                  );

        var user = await collection.Find(filter).FirstOrDefaultAsync();
        return user;
    }
    
    public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var filter = Builders<User>.Filter.Eq("RefreshToken", refreshToken);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }

    public bool IsUniqueUser(string username)
    {
        var filter = Builders<User>.Filter.Eq("UserName", username);
        var user = collection.Find(filter).FirstOrDefaultAsync();
        if (user.Result == null)
        {
            return true;
        }
        return false;
    }
    public async Task<Result<User>> Register(RegistrationRequest registrationRequest)
    {
        bool isUserNameUnique = IsUniqueUser(registrationRequest.UserName);
        if (!isUserNameUnique)
        {
            return Result.Fail<User>(ErrorMessages.UserAlreadyExists);
        }

        User user = new User
        {
            UserName = registrationRequest.UserName,
            Password = registrationRequest.Password,
            Email = registrationRequest.Email
        };
        if (user == null)
        {
            return Result.Fail<User>(ErrorMessages.ErrorWhileRegistering);
        }
        collection.InsertOne(user);
        user.Password = "";
        return Result.Ok(user);
    }
}
