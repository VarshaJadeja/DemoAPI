using Demo.Entities.ViewModels;
using Demo.Repositories.Constants;
using Demo.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly IUserService userService;
    private readonly IEncryptionService encryptionService;
    public HomeController(IEmailService emailService, IEncryptionService encryptionService, IUserService userService)
    {
        this.emailService = emailService;
        this.encryptionService = encryptionService;
        this.userService = userService;
    }

    public async Task<IActionResult> Index([FromQuery] string token, string expiration, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(expiration) || string.IsNullOrEmpty(email))
        {
            return BadRequest(ErrorMessages.ResetPasswordValidationError);
        }
        var decodedExpiration = encryptionService.Decode(expiration);
        var decodedToken = encryptionService.Decode(token);
        var decodedEmail = encryptionService.Decode(email);
        if (decodedExpiration == null || decodedToken == null || decodedEmail == null)
        {
            return BadRequest(ErrorMessages.InvalidCredentials);
        }
        var model = new ResetPasswordViewModel
        {
            Token = token,
            Email = email,
            Expiration = expiration,
        };
        var user = await userService.GetUserByEmailAsync(decodedEmail);
        if (user == null)
        {
            model.IsLinkValid = false;
            return BadRequest(ErrorMessages.UserNotFound);
        }
        if (DateTime.TryParseExact(decodedExpiration, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime expirationTime))
        {
            if (DateTime.Now < expirationTime)
            {
                if (user.Token != null && user.Token == token)
                {
                    if (user.IsPasswordUpdated)
                    {
                        model.IsPasswordUpdated = false;
                        model.IsLinkValid = false;
                    }
                    else
                    {
                        model.IsPasswordUpdated = false;
                        model.IsLinkValid = true;
                    }
                }
                else
                {
                    model.IsPasswordUpdated = false;
                    model.IsLinkValid = true;
                }

                return View("Index", model);
            }
            else
            {
                model.IsLinkValid = false;
                model.IsPasswordUpdated = false;
            }
        }
        return View(model);
    }


    [HttpPost("UpdatePassword")]
    public async Task<IActionResult> UpdatePassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        var decodedEmail = encryptionService.Decode(resetPasswordViewModel.Email!);
        var decodedToken = encryptionService.Decode(resetPasswordViewModel.Token!);
        var decodedExpiration = encryptionService.Decode(resetPasswordViewModel.Expiration!);
        var user = await userService.GetUserByEmailAsync(decodedEmail);

        if (user != null)
        {
            if (resetPasswordViewModel.ConfirmPassword == resetPasswordViewModel.NewPassword)
            {
                var updates = new Dictionary<string, object>
                 {
                     { "IsPasswordUpdated", "true" },
                     { "Token", resetPasswordViewModel.Token ! },
                     { "Password", resetPasswordViewModel.NewPassword }
                 };
                resetPasswordViewModel.IsLinkValid = false;
                resetPasswordViewModel.IsPasswordUpdated = true;
                await userService.UpdateFieldAsync(decodedEmail, updates);
                return View("Index", resetPasswordViewModel);
            }

        }

        return View("Index", resetPasswordViewModel);
    }
}
