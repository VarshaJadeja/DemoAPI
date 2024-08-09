using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using FluentResults;

namespace Demo.Services.Services
{
    public interface ITokenService
    {
        public string GenerateToken(User user);

        public Task<Result<RefreshResponse>> RefreshToken(string refreshToken);
    }
}