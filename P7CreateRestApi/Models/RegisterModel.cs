﻿using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and digits")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Fullname is required")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}