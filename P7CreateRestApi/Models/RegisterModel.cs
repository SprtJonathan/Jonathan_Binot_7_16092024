using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
        [RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "Le nom d'utilisateur ne peut contenir que des lettres, des chiffres, des tirets, des underscores et des points.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "L'adresse email est requise.")]
        [EmailAddress(ErrorMessage = "Le format de l'adresse email est invalide.")]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "L'adresse email ne peut contenir que des lettres, des chiffres et des caractères valides.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, dont une majuscule, un chiffre et un caractère spécial.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Le nom complet est requis.")]
        [RegularExpression("^[a-zA-Z-]+$", ErrorMessage = "Le nom complet ne peut contenir que des lettres et des tirets.")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Le rôle est requis.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Le rôle ne peut contenir que des lettres.")]
        public string Role { get; set; } = "user"; // Défaut "user"
    }

}