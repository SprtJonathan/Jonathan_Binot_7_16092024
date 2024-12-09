using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using P7CreateRestApi.Domain;
using System.Data;

namespace P7CreateRestApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(UserManager<User> userManager, IConfiguration configuration, ILogger<JwtService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateToken(User user)
        {
            _logger.LogInformation("Début de la génération du jeton JWT pour l'utilisateur {UserName} (ID: {UserId})", user.UserName, user.Id);

            // Récupérer les revendications de l'utilisateur
            var userClaims = await _userManager.GetClaimsAsync(user);
            _logger.LogInformation("Récupération des revendications pour l'utilisateur {UserName}. Total: {ClaimsCount}", user.UserName, userClaims.Count);

            // Récupérer les rôles de l'utilisateur
            var userRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Récupération des rôles pour l'utilisateur {UserName}. Total: {RolesCount}.", user.UserName, userRoles.Count);
            foreach (var role in userRoles)
                _logger.LogInformation("{UserName} a le rôle {UserRole}", user.UserName, role);

            // Ajouter les revendications et les rôles aux claims du jeton
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }
            .Union(userClaims);

            claims = claims.Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
            _logger.LogInformation("Les revendications et les rôles ont été ajoutés aux claims du jeton pour l'utilisateur {UserName}.", user.UserName);

            // Génération de la clé de sécurité
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            _logger.LogInformation("Clé de sécurité et informations d'identification de signature créées pour le jeton.");

            // Création du token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10), // Token expire après 10 minutes
                signingCredentials: creds);

            _logger.LogInformation("Le jeton JWT a été généré avec succès pour l'utilisateur {UserName}.", user.UserName);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
