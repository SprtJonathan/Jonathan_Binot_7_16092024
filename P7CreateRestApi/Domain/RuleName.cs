using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Controllers
{
    public class RuleName
    {
        [Required(ErrorMessage = "L'identifiant de la r�gle est requis.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la r�gle est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de la r�gle ne peut pas d�passer 100 caract�res.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La description de la r�gle est requise.")]
        [StringLength(250, ErrorMessage = "La description de la r�gle ne peut pas d�passer 250 caract�res.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le contenu JSON est requis.")]
        [StringLength(1000, ErrorMessage = "Le contenu JSON ne peut pas d�passer 1000 caract�res.")]
        public string Json { get; set; }

        [Required(ErrorMessage = "Le mod�le de r�gle est requis.")]
        [StringLength(500, ErrorMessage = "Le mod�le de r�gle ne peut pas d�passer 500 caract�res.")]
        public string Template { get; set; }

        [Required(ErrorMessage = "La cha�ne SQL est requise.")]
        [StringLength(1000, ErrorMessage = "La cha�ne SQL ne peut pas d�passer 1000 caract�res.")]
        public string SqlStr { get; set; }

        [Required(ErrorMessage = "La partie SQL est requise.")]
        [StringLength(1000, ErrorMessage = "La partie SQL ne peut pas d�passer 1000 caract�res.")]
        public string SqlPart { get; set; }
    }
}
