using Demo.Entities;
using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repositories.Repositories
{
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
        public bool IsUniqueUser(string username)
        {

            var filter = Builders<User>.Filter.Eq("UserName", username);
            var user = collection.Find(filter).FirstOrDefaultAsync();
            if(user.Result == null) 
            {
                return true;
            }
            return false;
        }

        public async Task<LoginReaponce> Login(LoginRequest loginRequest)
        {
            var filter = Builders<User>.Filter.And(
                            Builders<User>.Filter.Eq("UserName", loginRequest.UserName),
                            Builders<User>.Filter.Eq("Password", loginRequest.Password)
                        );
            var user = await collection.Find(filter).FirstOrDefaultAsync();
            if (user == null)
            {
               return new LoginReaponce()
                {
                    Token = "",
                    User = null
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginReaponce loginReaponce = new LoginReaponce()
            {
                Token = tokenHandler.WriteToken(token),
                User = user,
            };
            return loginReaponce;
        }

        public async Task<User> Register(RegistrationRequest registrationRequest)
        {
            User user = new()
            {
                UserName = registrationRequest.UserName,
                Password = registrationRequest.Password,
                Name = registrationRequest.Name,
                Role = registrationRequest.Role
            };

            collection.InsertOne(user);
            user.Password = "";
            return user;
        }
    }
}
