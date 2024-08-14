using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Demo.Repositories.Constants;
using Demo.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityUserController : ControllerBase
    {
        private readonly UserManager<NewAppUser> _userManager;
        private readonly SignInManager<NewAppUser> _signInManager;
        private readonly ITokenService tokenService;

        public IdentityUserController(UserManager<NewAppUser> userManager, SignInManager<NewAppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.tokenService = tokenService;
        }

        [HttpPost("register-identity")]
        public async Task<IActionResult> RegisterIdentity([FromBody] RegistrationRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new NewAppUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
                return Ok(new { message = ErrorMessages.RegisteredSuccessfully });

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return BadRequest(ModelState);
        }

        [HttpPost("login-identity")]
        public async Task<IActionResult> LoginIdentity([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    await _userManager.SetAuthenticationTokenAsync(user, "tokenProvider", "accesstoken", "");
                    return Ok(new { });
                }
                return Unauthorized();
            }

            if (result.IsLockedOut)
                return Forbid();

            return Unauthorized();
        }
    }
}
