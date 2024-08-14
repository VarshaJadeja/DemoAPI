using Demo.Entities.ViewModels;
using Demo.ExtensionMethod;
using Demo.Repositories.Constants;
using Demo.Repositories.Errors;
using Demo.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IUserService userService;
        private readonly IEncryptionService encryptionService;
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        private readonly IAuthService authService;

        public UserController(IProductService productService, IEmailService emailService, IUserService userService, IEncryptionService encryptionService,ITokenService tokenService, IAuthService authService)
        {
            this.productService = productService;
            this.emailService = emailService;
            this.userService = userService;
            this.encryptionService = encryptionService;
            this.authService = authService;
            this.tokenService = tokenService;
        }

        
        [HttpPost("login")]
        public async Task<IResult> Login([FromBody] LoginRequest model)
        {
            var loginResponse = await userService.Login(model);
            return loginResponse.Match(
            loginResponse => Results.Ok(loginResponse),
            errors => Errors.CreateResultFromErrors(errors.ToList()));
        } 

        [HttpPost("refreshToken")]
        public async Task<IResult> RefreshToken(string refreshToken)
        {
            var newAccessToken = await tokenService.RefreshToken(refreshToken);
            return newAccessToken.Match(
            newAccessToken => Results.Ok(newAccessToken),
            error => Results.BadRequest(error));
        }
      
        [HttpPost("register")]  
        public async Task<IResult> Register([FromBody]  RegistrationRequest model)
        {
            var user = await userService.Register(model);
            return user.Match(
             user => Results.Ok(user),
             error => Results.BadRequest(error)
            );
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetpasswordRequest(string toEmail)
        {
            var user = await userService.GetUserByEmailAsync(toEmail);
            if (user == null)
            {
                return BadRequest(ErrorMessages.UserNotFound);
            }
                var resetToken = authService.GenerateToken();
            var expiration = authService.CalculateExpirationTime();
            var resetLink = Url.Action(
                            "Index",
                            "Home",
                            new
                            {
                                token = encryptionService.Encode(resetToken),
                                expiration = encryptionService.Encode(expiration.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                                email = encryptionService.Encode(toEmail)
                            },
                            protocol: HttpContext.Request.Scheme);
            var subject = "Password Reset Request";
            var body = $"<p>Please reset your password by clicking <a href='{resetLink}'>this link</a>.</p>";

            await emailService.SendEmailToResetPasswordAsync(toEmail, subject, body);
            return Ok();
        }


    }
}
