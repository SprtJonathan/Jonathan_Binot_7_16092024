using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Domain;
using System.Threading.Tasks;
using P7CreateRestApi.Services;
using Microsoft.Extensions.Logging;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(SignInManager<User> signInManager, IJwtService tokenService, ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _jwtService = tokenService;
        }

        // POST api/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {


            // Attempt to sign in the user
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(model.UserName);
                var token = await _jwtService.GenerateToken(user);
                return Ok(new { Message = "Login successful.", Token = token });
            }


            return Unauthorized(new { Message = "Invalid login attempt." });
        }

        // POST api/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful." });
        }
    }
}