using Amazon.Runtime.Internal;
using Demo.Controllers;
using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Demo.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test;

public class UserControllertest
{
    private readonly UserController _controller;
    private readonly Mock<IProductService> _productService;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IEncryptionService> _encryptionService;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IAuthService> _authService;

    public UserControllertest()
    {
        _productService = new Mock<IProductService>();
        _userServiceMock = new Mock<IUserService>();
        _emailService = new Mock<IEmailService>();
        _encryptionService = new Mock<IEncryptionService>();
        _authService = new Mock<IAuthService>();
        _controller = new UserController(_productService.Object, _emailService.Object, _userServiceMock.Object, _encryptionService.Object, _authService.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var validLoginRequest = new LoginRequest { UserName = "validuser", Password = "validpassword" };
        var expectedLoginResponse = new LoginResponse
        {
            User = new User { UserName = "validuser", Password = "validpassword" },
            Token = "mockedtoken"
        };
        _userServiceMock.Setup(x => x.Login(validLoginRequest)).ReturnsAsync(expectedLoginResponse);
        _emailService.Setup(x => x.SendEmailToResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Login(validLoginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);
        //Assert.Equal(expectedLoginResponse.User, loginResponse.User);
        Assert.Equal("validuser", loginResponse.User.UserName);
        Assert.Equal("validpassword", loginResponse.User.Password);
        Assert.Equal(expectedLoginResponse.Token, loginResponse.Token);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var invalidLoginRequest = new LoginRequest { UserName = "invaliduser", Password = "invalidpassword" };
        var expectedErrorMessage = "Username or password is incorrect";
        _userServiceMock.Setup(x => x.Login(invalidLoginRequest)).ReturnsAsync(new LoginResponse());

        // Act
        var result = await _controller.Login(invalidLoginRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        // Use reflection to access properties of the anonymous type
        var response = badRequestResult.Value as object;
        var messageProperty = response.GetType().GetProperty("message");
        var message = messageProperty.GetValue(response, null).ToString();

        Assert.Equal(expectedErrorMessage, message);
    }



    [Fact]
    public async Task Register_ValidModel_ReturnsOk()
    {
        // Arrange
        var validRegistrationRequest = new RegistrationRequest { UserName = "newuser", Password = "password" };
        //_userServiceMock.Setup(x => x.IsUniqueUser(validRegistrationRequest.UserName)).Returns(true);
        _userServiceMock.Setup(x => x.Register(validRegistrationRequest)).ReturnsAsync(new User());

        // Act
        var result = await _controller.Register(validRegistrationRequest);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Register_DuplicateUser_ReturnsBadRequest()
    {
        // Arrange
        var duplicateRegistrationRequest = new RegistrationRequest { UserName = "vrs", Password = "123" };
        //_userServiceMock.Setup(x => x.IsUniqueUser(duplicateRegistrationRequest.UserName)).Returns(false);

        // Act
        var result = await _controller.Register(duplicateRegistrationRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        // Handle as string if the response is a string
        var response = badRequestResult.Value as object;
        var messageProperty = response.GetType().GetProperty("message");
        var message = messageProperty.GetValue(response, null).ToString();

        Assert.Equal("User already exists", message);

    }

    [Fact]
    public async Task Register_ErrorWhileRegistering_ReturnsBadRequest()
    {
        // Arrange
        var errorRegistrationRequest = new RegistrationRequest { UserName = "dfdsfs", Password = "dsf" };
        //_userServiceMock.Setup(x => x.IsUniqueUser(errorRegistrationRequest.UserName)).Returns(true);
        _userServiceMock.Setup(x => x.Register(errorRegistrationRequest)).ReturnsAsync((User)null);

        // Act
        var result = await _controller.Register(errorRegistrationRequest);

        // Assert
        //var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //var errorObject = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        //Assert.Equal("Error while registering", errorObject["message"]);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);

        Assert.Equal("Error while registering", response.Message);
    }
    [Fact]
    public async Task Registration_ReturnsBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var registrationRequest = new RegistrationRequest { UserName = "new-user" };
        //_userServiceMock.Setup(repo => repo.IsUniqueUser(registrationRequest.UserName))
        //             .Returns(true);
        _userServiceMock.Setup(repo => repo.Register(registrationRequest))
                     .ReturnsAsync(null as User);

        // Act
        var result = await _controller.Register(registrationRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);

        Assert.Equal("Error while registering", response.Message);
    }

}
