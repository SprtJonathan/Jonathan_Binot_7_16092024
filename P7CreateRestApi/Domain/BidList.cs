using System;
using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class BidList
    {
        [Required(ErrorMessage = "L'identifiant du Bid est requis.")]
        public int BidListId { get; set; }

        [Required(ErrorMessage = "Le compte est requis.")]
        [StringLength(50, ErrorMessage = "Le compte ne peut pas d�passer 50 caract�res.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Le type de Bid est requis.")]
        [StringLength(50, ErrorMessage = "Le type de Bid ne peut pas d�passer 50 caract�res.")]
        public string BidType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantit� de Bid doit �tre un nombre positif.")]
        public double? BidQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantit� demand�e doit �tre un nombre positif.")]
        public double? AskQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix du Bid doit �tre un nombre positif.")]
        public double? Bid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix demand� doit �tre un nombre positif.")]
        public double? Ask { get; set; }

        [Required(ErrorMessage = "Le benchmark est requis.")]
        [StringLength(100, ErrorMessage = "Le benchmark ne peut pas d�passer 100 caract�res.")]
        public string Benchmark { get; set; }

        public DateTime? BidListDate { get; set; }

        [Required(ErrorMessage = "Le commentaire est requis.")]
        [StringLength(250, ErrorMessage = "Le commentaire ne peut pas d�passer 250 caract�res.")]
        public string Commentary { get; set; }

        [Required(ErrorMessage = "La s�curit� du Bid est requise.")]
        [StringLength(50, ErrorMessage = "La s�curit� du Bid ne peut pas d�passer 50 caract�res.")]
        public string BidSecurity { get; set; }

        [Required(ErrorMessage = "Le statut du Bid est requis.")]
        [StringLength(50, ErrorMessage = "Le statut du Bid ne peut pas d�passer 50 caract�res.")]
        public string BidStatus { get; set; }

        [Required(ErrorMessage = "Le trader est requis.")]
        [StringLength(50, ErrorMessage = "Le nom du trader ne peut pas d�passer 50 caract�res.")]
        public string Trader { get; set; }

        [Required(ErrorMessage = "Le nom du livre est requis.")]
        [StringLength(50, ErrorMessage = "Le nom du livre ne peut pas d�passer 50 caract�res.")]
        public string Book { get; set; }

        [Required(ErrorMessage = "Le nom de cr�ation est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de cr�ation ne peut pas d�passer 100 caract�res.")]
        public string CreationName { get; set; }

        public DateTime? CreationDate { get; set; }

        [Required(ErrorMessage = "Le nom de r�vision est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de r�vision ne peut pas d�passer 100 caract�res.")]
        public string RevisionName { get; set; }

        public DateTime? RevisionDate { get; set; }

        [Required(ErrorMessage = "Le nom de l'affaire est requis.")]
        [StringLength(100, ErrorMessage = "Le nom de l'affaire ne peut pas d�passer 100 caract�res.")]
        public string DealName { get; set; }

        [Required(ErrorMessage = "Le type d'affaire est requis.")]
        [StringLength(50, ErrorMessage = "Le type d'affaire ne peut pas d�passer 50 caract�res.")]
        public string DealType { get; set; }

        [Required(ErrorMessage = "L'identifiant de la source est requis.")]
        [StringLength(50, ErrorMessage = "L'identifiant de la source ne peut pas d�passer 50 caract�res.")]
        public string SourceListId { get; set; }

        [Required(ErrorMessage = "Le c�t� est requis.")]
        [StringLength(50, ErrorMessage = "Le c�t� ne peut pas d�passer 50 caract�res.")]
        public string Side { get; set; }
    }
}
