using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel register)
        {
            _logger.LogInformation("Début de la tentative d'enregistrement d'un nouvel utilisateur.");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Échec de la validation du modèle lors de l'enregistrement d'un utilisateur.");
                return BadRequest(ModelState);
            }

            // Vérifier si le rôle existe, sinon le créer
            if (!await _roleManager.RoleExistsAsync(register.Role))
            {
                _logger.LogWarning("Le rôle {Role} n'existe pas. Tentative de création du rôle.", register.Role);
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
                if (!createRoleResult.Succeeded)
                {
                    _logger.LogError("Impossible de créer le rôle {Role}. Erreurs: {Errors}", register.Role, string.Join(", ", createRoleResult.Errors.Select(e => e.Description)));
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create role.");
                }
                _logger.LogInformation("Rôle {Role} créé avec succès.", register.Role);
            }
            else
            {
                _logger.LogInformation("Le rôle {Role} existe déjà.", register.Role);
            }

            var user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
                Fullname = register.Fullname,
                Role = register.Role
            };

            // Créer l'utilisateur avec un mot de passe
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Utilisateur {UserName} créé avec succès. Tentative d'ajout de l'utilisateur au rôle {Role}.", register.UserName, register.Role);
                await _userManager.AddToRoleAsync(user, register.Role);
                _logger.LogInformation("Utilisateur {UserName} ajouté au rôle {Role} avec succès.", register.UserName, register.Role);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }

            // Log les erreurs de création d'utilisateur
            var errors = result.Errors.Select(e => e.Description);
            _logger.LogError("Échec de la création de l'utilisateur {UserName}. Erreurs: {Errors}", register.UserName, string.Join(", ", errors));

            return BadRequest(new { Errors = errors });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            _logger.LogInformation("Tentative de récupération de l'utilisateur avec ID {UserId}.", id);

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur avec ID {UserId} non trouvé.", id);
                return NotFound();
            }

            _logger.LogInformation("Utilisateur avec ID {UserId} récupéré avec succès.", id);
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Récupération de la liste de tous les utilisateurs.");

            var users = await _userRepository.GetAllUsersAsync();

            _logger.LogInformation("{UserCount} utilisateurs récupérés avec succès.", users.Count());
            return Ok(users);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterModel register)
        {
            _logger.LogInformation("Tentative de mise à jour de l'utilisateur avec ID {UserId}.", id);

            if (id <= 0 || register == null)
            {
                _logger.LogWarning("ID invalide ou corps de la requête nul pour la mise à jour de l'utilisateur avec ID {UserId}.", id);
                return BadRequest("Invalid ID or request body.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Utilisateur avec ID {UserId} non trouvé pour la mise à jour.", id);
                    return NotFound();
                }

                user.UserName = register.UserName;
                user.Email = register.Email;
                user.Fullname = register.Fullname;

                if (user.Role != register.Role)
                {
                    _logger.LogInformation("Modification du rôle de l'utilisateur {UserName} de {OldRole} à {NewRole}.", user.UserName, user.Role, register.Role);

                    await _userManager.RemoveFromRoleAsync(user, user.Role);
                    if (!await _roleManager.RoleExistsAsync(register.Role))
                    {
                        _logger.LogInformation("Le rôle {Role} n'existe pas. Tentative de création du rôle.", register.Role);
                        await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
                    }
                    await _userManager.AddToRoleAsync(user, register.Role);
                    user.Role = register.Role;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur avec ID {UserId} mis à jour avec succès.", id);
                    return Ok(user);
                }

                var errors = result.Errors.Select(e => e.Description);
                _logger.LogError("Échec de la mise à jour de l'utilisateur avec ID {UserId}. Erreurs: {Errors}", id, string.Join(", ", errors));
                return BadRequest(new { Errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur avec ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Tentative de suppression de l'utilisateur avec ID {UserId}.", id);

            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("Utilisateur avec ID {UserId} non trouvé pour la suppression.", id);
                    return NotFound();
                }

                var result = await _userRepository.DeleteUserAsync(id);
                if (!result)
                {
                    _logger.LogError("Échec de la suppression de l'utilisateur avec ID {UserId}.", id);
                    return BadRequest("Failed to delete user.");
                }

                _logger.LogInformation("Utilisateur avec ID {UserId} supprimé avec succès.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur avec ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }


    }
}