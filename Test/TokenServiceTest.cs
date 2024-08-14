using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Demo.Entities.Entities;
using Demo.Repositories;
using Demo.Services.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Tests
{
    public class TokenServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _tokenService;
        private const string SecretKey = "SuperSecretKey1234555555555555555";

        public TokenServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            var SectionMock = new Mock<IConfigurationSection>();
            SectionMock.Setup(s => s.Value).Returns(SecretKey);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c.GetSection("ApiSetting:Secret")).Returns(SectionMock.Object);
            _tokenService = new TokenService(_mockUserRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidToken()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = It.IsAny<string>()
            };

            var token = _tokenService.GenerateToken(user);
            
            token.Should().NotBeNullOrWhiteSpace();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
            jwtToken.Claims.Should().Contain(claim => (claim.Type == ClaimTypes.Name && claim.Value == user.Id) || (claim.Type == ClaimTypes.Expiration));
        }

        [Fact]
        public void TestTokenValidation()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = It.IsAny<string>()
            };

            var token = _tokenService.GenerateToken(user);

            // Validate the token claims
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            // Extract and validate the claims
            Assert.True(principal.HasClaim(c => c.Type == ClaimTypes.Name && c.Value == user.Id.ToString()));
            Assert.True(principal.HasClaim(c => c.Type == ClaimTypes.Expiration && DateTime.Parse(c.Value) > DateTime.UtcNow));
            Assert.NotNull(validatedToken);
        }
        [Fact]
        public async Task RefreshToken_ShouldReturnNewAccessToken_WhenRefreshTokenIsValid()
        {
            var refreshToken = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = It.IsAny<string>(),
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(5)
            };
            _mockUserRepository.Setup(repo => repo.GetUserByRefreshTokenAsync(refreshToken))
                               .ReturnsAsync(user);

            var result = await _tokenService.RefreshToken(refreshToken);

            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().NotBeNullOrWhiteSpace();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(result.Value.Token);
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
            jwtToken.Claims.Should().Contain(claim => (claim.Type == ClaimTypes.Name && claim.Value == user.Id) || (claim.Type == ClaimTypes.Expiration));
        }

        [Fact]
        public async Task RefreshToken_ShouldFail_WhenUserNotFound()
        {
            var refreshToken = Guid.NewGuid().ToString();
            _mockUserRepository.Setup(repo => repo.GetUserByRefreshTokenAsync(refreshToken))
                               .ReturnsAsync(() => null);

            var result = await _tokenService.RefreshToken(refreshToken);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Contains("User Not Found"));
        }

        [Fact]
        public async Task RefreshToken_ShouldFail_WhenRefreshTokenIsExpired()
        {
            var refreshToken = Guid.NewGuid().ToString();
            var user = new User
            {
                Email = It.IsAny<string>(),
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(-5) 
            };
            _mockUserRepository.Setup(repo => repo.GetUserByRefreshTokenAsync(refreshToken))
                               .ReturnsAsync(user);

            var result = await _tokenService.RefreshToken(refreshToken);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Contains("Token Expired"));
        }
    }
}
