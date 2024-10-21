using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifier si le rôle existe, sinon le créer
            if (!await _roleManager.RoleExistsAsync(register.Role))
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
                if (!createRoleResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create role.");
                }
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
                await _userManager.AddToRoleAsync(user, register.Role);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterModel register)
        {
            if (id <= 0 || register == null)
            {
                return BadRequest("Invalid ID or request body.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = register.UserName;
                user.Email = register.Email;
                user.Fullname = register.Fullname;

                // Gérer le changement de rôle
                if (user.Role != register.Role)
                {
                    await _userManager.RemoveFromRoleAsync(user, user.Role);
                    if (!await _roleManager.RoleExistsAsync(register.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
                    }
                    await _userManager.AddToRoleAsync(user, register.Role);
                    user.Role = register.Role;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(user);
                }

                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                var result = await _userRepository.DeleteUserAsync(id);
                if (!result)
                {
                    return BadRequest("Failed to delete user.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

    }
}