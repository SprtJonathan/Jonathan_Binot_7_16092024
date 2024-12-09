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
            _logger = logger;
        }

        // POST api/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation("Tentative de connexion pour l'utilisateur {UserName}.", model.UserName);

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Connexion r�ussie pour l'utilisateur {UserName}.", model.UserName);

                var user = await _signInManager.UserManager.FindByNameAsync(model.UserName);
                var token = await _jwtService.GenerateToken(user);

                _logger.LogInformation("Token JWT g�n�r� avec succ�s pour l'utilisateur {UserName}.", model.UserName);
                return Ok(new ApiResponse { Message = "Connexion r�ussie.", Token = token });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("L'utilisateur {UserName} est verrouill�.", model.UserName);
                return Unauthorized(new ApiResponse { Message = "Compte verrouill� en raison de tentatives de connexion infructueuses." });
            }

            _logger.LogWarning("Tentative de connexion non valide pour l'utilisateur {UserName}.", model.UserName);
            return Unauthorized(new ApiResponse { Message = "Nom d'utilisateur ou mot de passe incorrect." });
        }

        // POST api/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new ApiResponse { Message = "Token manquant." });
            }
            _logger.LogInformation("D�connexion de l'utilisateur.");

            // R�voquer le token
            var tokenRevocationService = HttpContext.RequestServices.GetRequiredService<ITokenRevocationService>();
            await tokenRevocationService.RevokeTokenAsync(token);

            _logger.LogInformation("Token r�voqu� : {Token} - D�connexion r�ussie", token);

            return Ok(new ApiResponse { Message = "D�connexion r�ussie." });
        }
    }
}
