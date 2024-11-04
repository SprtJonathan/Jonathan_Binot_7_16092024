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

            // Tentative de connexion de l'utilisateur
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Connexion réussie pour l'utilisateur {UserName}.", model.UserName);

                var user = await _signInManager.UserManager.FindByNameAsync(model.UserName);
                var token = await _jwtService.GenerateToken(user);

                _logger.LogInformation("Token JWT généré avec succès pour l'utilisateur {UserName}.", model.UserName);
                return Ok(new { Message = "Connexion réussie.", Token = token });
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("L'utilisateur {UserName} est verrouillé en raison de tentatives de connexion infructueuses.", model.UserName);
                return Unauthorized(new { Message = "Compte verrouillé en raison de tentatives de connexion infructueuses." });
            }

            _logger.LogWarning("Tentative de connexion non valide pour l'utilisateur {UserName}.", model.UserName);
            return Unauthorized(new { Message = "Tentative de connexion non valide." });
        }

        // POST api/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Déconnexion de l'utilisateur.");

            await _signInManager.SignOutAsync();

            _logger.LogInformation("Déconnexion réussie.");
            return Ok(new { Message = "Déconnexion réussie." });
        }
    }
}