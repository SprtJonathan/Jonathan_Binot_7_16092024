using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class Rating
    {
        [Required(ErrorMessage = "L'identifiant de la notation est requis.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La notation Moody's est requise.")]
        [StringLength(10, ErrorMessage = "La notation Moody's ne peut pas dépasser 10 caractères.")]
        public string MoodysRating { get; set; }

        [Required(ErrorMessage = "La notation S&P est requise.")]
        [StringLength(10, ErrorMessage = "La notation S&P ne peut pas dépasser 10 caractères.")]
        public string SandPRating { get; set; }

        [Required(ErrorMessage = "La notation Fitch est requise.")]
        [StringLength(10, ErrorMessage = "La notation Fitch ne peut pas dépasser 10 caractères.")]
        public string FitchRating { get; set; }

        [Range(0, 255, ErrorMessage = "Le numéro d'ordre doit être compris entre 0 et 255.")]
        public byte? OrderNumber { get; set; }
    }
}
