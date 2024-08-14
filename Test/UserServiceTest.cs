using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Demo.Repositories;
using Demo.Services.Services;
using FluentResults;
using Moq;
using Xunit;
using FluentAssertions;

namespace Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _userService = new UserService(_mockUserRepository.Object, _mockTokenService.Object);
        }
        [Fact]
        public async Task Register_ShouldReturnSuccess_WhenUserIsRegistered()
        {
            // Arrange
            var registrationRequest = new RegistrationRequest { UserName = "newuser", Password = "password", Email = "newuser@example.com" };
            var user = new User { Email = registrationRequest.Email, UserName = registrationRequest.UserName };
            _mockUserRepository.Setup(repo => repo.Register(registrationRequest))
                               .ReturnsAsync(Result.Ok(user));

            // Act
            var result = await _userService.Register(registrationRequest);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(user);
        }
         [Fact]
        public async Task Register_ShouldReturnFailure_WhenUserRegistrationFails()
        {
            var registrationRequest = new RegistrationRequest { UserName = It.IsAny<string>(), Password = It.IsAny<string>(), Email = It.IsAny<string>() };
            _mockUserRepository.Setup(repo => repo.Register(registrationRequest))
                               .ReturnsAsync(Result.Fail<User>("Registration failed"));

            var result = await _userService.Register(registrationRequest);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Contains("Registration failed"));
        }

        [Fact]
        public async Task Login_ShouldReturnValidResponse_WhenCredentialsAreValid()
        {
            var loginRequest = new LoginRequest { UserName = It.IsAny<string>(), Password = It.IsAny<string>() };
            var user = new User { Email = It.IsAny<string>(), UserName = It.IsAny<string>() };
            _mockUserRepository.Setup(repo => repo.GetUser(loginRequest))
                               .ReturnsAsync(user);
            _mockTokenService.Setup(service => service.GenerateToken(user))
                             .Returns("token");

            var result = await _userService.Login(loginRequest);

            result.IsSuccess.Should().BeTrue();
            var loginResponse = result.Value;
            loginResponse.RefreshToken.Should().NotBeNullOrEmpty();
            loginResponse.Token.Should().NotBeNullOrEmpty();    
            loginResponse.User.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenUserNotFound()
        {
            var loginRequest = new LoginRequest {UserName = It.IsAny<string>(), Password = It.IsAny<string>() };
            _mockUserRepository.Setup(repo => repo.GetUser(loginRequest))
                               .ReturnsAsync(() => null);

            var result = await _userService.Login(loginRequest);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Contains("Invalid Credentials"));
        }

        [Fact]
        public async Task Login_ShouldUpdateUserWithRefreshToken_WhenCredentialsAreValid()
        {
            var loginRequest = new LoginRequest { UserName = It.IsAny<string>(), Password = It.IsAny<string>() };
            var user = new User { Email = It.IsAny<string>(), UserName = It.IsAny<string>() };
            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(3);

            _mockUserRepository.Setup(repo => repo.GetUser(loginRequest))
                               .ReturnsAsync(user);
            _mockTokenService.Setup(service => service.GenerateToken(user))
                             .Returns(It.IsAny<string>());
            _mockUserRepository.Setup(repo => repo.UpdateFieldsAsync(user.Email, It.IsAny<Dictionary<string, object>>()))
                               .ReturnsAsync(true);

            var result = await _userService.Login(loginRequest);

            _mockUserRepository.Verify(repo => repo.UpdateFieldsAsync(user.Email, It.Is<Dictionary<string, object>>(dict =>
                dict.ContainsKey("RefreshToken") && dict.ContainsKey("RefreshTokenExpiry"))),
                Times.Once);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            var email = It.IsAny<string>();
            var user = new User { Email = email, UserName = It.IsAny<string>() };
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(email))
                               .ReturnsAsync(user);

            var result = await _userService.GetUserByEmailAsync(email);

            result.Should().BeEquivalentTo(user);
        }
        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var email = It.IsAny<string>();
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(email))
                               .ReturnsAsync(() => null);

            var result = await _userService.GetUserByEmailAsync(email);

            result.Should().BeNull();
        }
    }
}
