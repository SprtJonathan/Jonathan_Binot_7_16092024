using System;
using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class BidList
    {
        [Required(ErrorMessage = "L'identifiant du Bid est requis.")]
        public int BidListId { get; set; }

        [Required(ErrorMessage = "Le compte est requis.")]
        [StringLength(50, ErrorMessage = "Le compte ne peut pas dépasser 50 caractères.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Le type de Bid est requis.")]
        [StringLength(50, ErrorMessage = "Le type de Bid ne peut pas dépasser 50 caractères.")]
        public string BidType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantité de Bid doit être un nombre positif.")]
        public double? BidQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantité demandée doit être un nombre positif.")]
        public double? AskQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix du Bid doit être un nombre positif.")]
        public double? Bid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix demandé doit être un nombre positif.")]
        public double? Ask { get; set; }

        [Required(ErrorMessage = "Le benchmark est requis.")]
        [StringLength(100, ErrorMessage = "Le benchmark ne peut pas dépasser 100 caractères.")]
        public string Benchmark { get; set; }

        public DateTime? BidListDate { get; set; }

        [Required(ErrorMessage = "Le commentaire est requis.")]
        [StringLength(250, ErrorMessage = "Le commentaire ne peut pas dépasser 250 caractères.")]
        public string Commentary { get; set; }

        [Required(ErrorMessage = "La sécurité du Bid est requise.")]
        [StringLength(50, ErrorMessage = "La sécurité du Bid ne peut pas dépasser 50 caractères.")]
        public string BidSecurity { get; set; }

        [Required(ErrorMessage = "Le statut du Bid est requis.")]
        [StringLength(50, ErrorMessage = "Le statut du Bid ne peut pas dépasser 50 caractères.")]
        public string BidStatus { get; set; }

        [Required(ErrorMessage = "Le trader est requis.")]
        [StringLength(50, ErrorMessage = "Le nom du trader ne peut pas dépasser 50 caractères.")]
        public string Trader { get; set; }

        [Required(ErrorMessage = "Le nom du livre est requis.")]
        [StringLength(50, ErrorMessage = "Le nom du livre ne peut pas dépasser 50 caractères.")]
        public string Book { get; set; }

        [Required(ErrorMessage = "Le nom de création est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de création ne peut pas dépasser 100 caractères.")]
        public string CreationName { get; set; }

        public DateTime? CreationDate { get; set; }

        [Required(ErrorMessage = "Le nom de révision est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de révision ne peut pas dépasser 100 caractères.")]
        public string RevisionName { get; set; }

        public DateTime? RevisionDate { get; set; }

        [Required(ErrorMessage = "Le nom de l'affaire est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de l'affaire ne peut pas dépasser 100 caractères.")]
        public string DealName { get; set; }

        [Required(ErrorMessage = "Le type d'affaire est requis.")]
        [StringLength(50, ErrorMessage = "Le type d'affaire ne peut pas dépasser 50 caractères.")]
        public string DealType { get; set; }

        [Required(ErrorMessage = "L'identifiant de la source est requis.")]
        [StringLength(50, ErrorMessage = "L'identifiant de la source ne peut pas dépasser 50 caractères.")]
        public string SourceListId { get; set; }

        [Required(ErrorMessage = "Le côté est requis.")]
        [StringLength(50, ErrorMessage = "Le côté ne peut pas dépasser 50 caractères.")]
        public string Side { get; set; }
    }
}
