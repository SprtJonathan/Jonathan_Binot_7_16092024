using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Controllers
{
    public class RuleName
    {
        [Required(ErrorMessage = "L'identifiant de la règle est requis.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la règle est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de la règle ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La description de la règle est requise.")]
        [StringLength(250, ErrorMessage = "La description de la règle ne peut pas dépasser 250 caractères.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le contenu JSON est requis.")]
        [StringLength(1000, ErrorMessage = "Le contenu JSON ne peut pas dépasser 1000 caractères.")]
        public string Json { get; set; }

        [Required(ErrorMessage = "Le modèle de règle est requis.")]
        [StringLength(500, ErrorMessage = "Le modèle de règle ne peut pas dépasser 500 caractères.")]
        public string Template { get; set; }

        [Required(ErrorMessage = "La chaîne SQL est requise.")]
        [StringLength(1000, ErrorMessage = "La chaîne SQL ne peut pas dépasser 1000 caractères.")]
        public string SqlStr { get; set; }

        [Required(ErrorMessage = "La partie SQL est requise.")]
        [StringLength(1000, ErrorMessage = "La partie SQL ne peut pas dépasser 1000 caractères.")]
        public string SqlPart { get; set; }
    }
}
