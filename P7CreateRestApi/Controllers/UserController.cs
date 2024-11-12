using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// R�cup�ration de tous les utilisateurs
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Tentative de r�cup�ration de tous les utilisateurs.");

            var users = await _userRepository.GetAllUsersAsync();
            if (users == null || !users.Any())
            {
                _logger.LogWarning("Aucun utilisateur trouv�.");
                return Ok(new List<User>());
            }

            _logger.LogInformation("{UserCount} utilisateurs r�cup�r�s avec succ�s.", users.Count());
            return Ok(users);
        }

        /// <summary>
        /// R�cup�ration d'un utilisateur par son Id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserById(int id)
        {
            _logger.LogInformation("Tentative de r�cup�ration de l'utilisateur avec ID {UserId}.", id);

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur avec ID {UserId} non trouv�.", id);
                return NotFound();
            }

            _logger.LogInformation("Utilisateur avec ID {UserId} r�cup�r� avec succ�s.", id);
            return Ok(user);
        }

        /// <summary>
        /// Enregistrement d'un nouvel utilisateur
        /// </summary>
        [HttpPost("register")]        
        public async Task<IActionResult> Register([FromBody] RegisterModel register)
        {
            _logger.LogInformation("D�but de la tentative d'enregistrement d'un nouvel utilisateur.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour l'enregistrement de l'utilisateur.");
                return BadRequest(ModelState);
            }

            // V�rifier si le r�le existe, sinon le cr�er
            if (!await _roleManager.RoleExistsAsync(register.Role))
            {
                _logger.LogWarning("Le r�le {Role} n'existe pas. Cr�ation du r�le.", register.Role);
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
                if (!createRoleResult.Succeeded)
                {
                    _logger.LogError("Impossible de cr�er le r�le {Role}. Erreurs: {Errors}", register.Role, string.Join(", ", createRoleResult.Errors.Select(e => e.Description)));
                    return StatusCode(500, "Impossible de cr�er le r�le.");
                }
                _logger.LogInformation("R�le {Role} cr�� avec succ�s.", register.Role);
            }

            var user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
                Fullname = register.Fullname,
                Role = register.Role
            };

            // Cr�er l'utilisateur avec un mot de passe
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Utilisateur {UserName} cr�� avec succ�s.", register.UserName);
                await _userManager.AddToRoleAsync(user, register.Role);
                _logger.LogInformation("Utilisateur {UserName} ajout� au r�le {Role}.", register.UserName, register.Role);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }

            // Log les erreurs de cr�ation d'utilisateur
            var errors = result.Errors.Select(e => e.Description);
            _logger.LogError("�chec de la cr�ation de l'utilisateur {UserName}. Erreurs: {Errors}", register.UserName, string.Join(", ", errors));
            return BadRequest(new { Errors = errors });
        }

        /// <summary>
        /// Mise � jour d'un utilisateur
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterModel register)
        {
            _logger.LogInformation("Tentative de mise � jour de l'utilisateur avec ID {UserId}.", id);

            if (id <= 0 || register == null)
            {
                _logger.LogWarning("ID invalide ou corps de la requ�te nul pour la mise � jour de l'utilisateur avec ID {UserId}.", id);
                return BadRequest("ID invalide ou corps de la requ�te nul.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Utilisateur avec ID {UserId} non trouv� pour la mise � jour.", id);
                    return NotFound();
                }

                user.UserName = register.UserName;
                user.Email = register.Email;
                user.Fullname = register.Fullname;

                if (user.Role != register.Role)
                {
                    _logger.LogInformation("Modification du r�le de l'utilisateur {UserName} de {OldRole} � {NewRole}.", user.UserName, user.Role, register.Role);

                    await _userManager.RemoveFromRoleAsync(user, user.Role);
                    if (!await _roleManager.RoleExistsAsync(register.Role))
                    {
                        _logger.LogInformation("Le r�le {Role} n'existe pas. Cr�ation du r�le.", register.Role);
                        await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
                    }
                    await _userManager.AddToRoleAsync(user, register.Role);
                    user.Role = register.Role;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur avec ID {UserId} mis � jour avec succ�s.", id);
                    return Ok(user);
                }

                var errors = result.Errors.Select(e => e.Description);
                _logger.LogError("�chec de la mise � jour de l'utilisateur avec ID {UserId}. Erreurs: {Errors}", id, string.Join(", ", errors));
                return BadRequest(new { Errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise � jour de l'utilisateur avec ID {UserId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Suppression d'un utilisateur
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Tentative de suppression de l'utilisateur avec ID {UserId}.", id);

            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("Utilisateur avec ID {UserId} non trouv� pour la suppression.", id);
                    return NotFound();
                }

                var result = await _userRepository.DeleteUserAsync(id);
                if (!result)
                {
                    _logger.LogError("�chec de la suppression de l'utilisateur avec ID {UserId}.", id);
                    return BadRequest("�chec de la suppression de l'utilisateur.");
                }

                _logger.LogInformation("Utilisateur avec ID {UserId} supprim� avec succ�s.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur avec ID {UserId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
