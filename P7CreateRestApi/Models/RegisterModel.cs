using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression("^[a-zA-Z0-9_-]{3,}$", ErrorMessage = "Username cannot contain special characters and must be at least 3 characters long")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression("^(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$", ErrorMessage = "Username cannot contain special characters and must be at least 3 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Fullname is required")]
        [RegularExpression("^[a-zA-Z]{3,}$", ErrorMessage = "Name cannot contain numbers, special characters, or be less than 3 characters")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}