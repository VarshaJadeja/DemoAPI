using Demo.Entities.ViewModels;
using Demo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Demo.Controllers
{
    [ApiController]
   
    public class UserController : Controller
    {
        private readonly IProductService productService;
        //protected ApiResponse _response;
        public UserController(IProductService productService) =>
            this.productService = productService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var LoginResponse = await productService.Login(model);
            if(LoginResponse.User == null || string.IsNullOrEmpty(LoginResponse.Token))
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(LoginResponse);
        }

        [HttpPost("register")]  
        public async Task<IActionResult> Register([FromBody]  RegistrationRequest model)
        {
            bool isUserNameUnique = productService.IsUniqueUser(model.UserName);
            if(!isUserNameUnique)
            {
                return BadRequest(new { message = "User alredy exist" });
            }
            var user = productService.Register(model);
            if(user == null)
            {
                return BadRequest(new { message = "Error while registering" });
            }
            return Ok();
        }
    }
}
