using System;
using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class CurvePoint
    {
        [Required(ErrorMessage = "L'identifiant du point de courbe est requis.")]
        public int Id { get; set; }

        [Range(0, 255, ErrorMessage = "L'identifiant de la courbe doit être compris entre 0 et 255.")]
        public byte? CurveId { get; set; }

        public DateTime? AsOfDate { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue, ErrorMessage = "La durée doit être un nombre positif.")]
        public double? Term { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La valeur du point de courbe doit être un nombre positif.")]
        public double? CurvePointValue { get; set; }

        public DateTime? CreationDate { get; set; } = DateTime.Now;
    }
}
