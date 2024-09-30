using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression("^[a-zA-Z0-9_-]{3,}$", ErrorMessage = "Username cannot contain special characters and must be at least 3 characters long")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}