using System;
using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class Trade
    {
        [Required(ErrorMessage = "L'identifiant du trade est requis.")]
        public int TradeId { get; set; }

        [Required(ErrorMessage = "Le compte est requis.")]
        [StringLength(50, ErrorMessage = "Le compte ne peut pas dépasser 50 caractères.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Le type de compte est requis.")]
        [StringLength(50, ErrorMessage = "Le type de compte ne peut pas dépasser 50 caractères.")]
        public string AccountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantité achetée doit être un nombre positif.")]
        public double? BuyQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantité vendue doit être un nombre positif.")]
        public double? SellQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix d'achat doit être un nombre positif.")]
        public double? BuyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix de vente doit être un nombre positif.")]
        public double? SellPrice { get; set; }

        [DataType(DataType.Date, ErrorMessage = "La date de trade doit être au format date.")]
        public DateTime? TradeDate { get; set; }

        [StringLength(50, ErrorMessage = "La sécurité du trade ne peut pas dépasser 50 caractères.")]
        public string TradeSecurity { get; set; }

        [StringLength(50, ErrorMessage = "Le statut du trade ne peut pas dépasser 50 caractères.")]
        public string TradeStatus { get; set; }

        [StringLength(50, ErrorMessage = "Le nom du trader ne peut pas dépasser 50 caractères.")]
        public string Trader { get; set; }

        [StringLength(100, ErrorMessage = "Le benchmark ne peut pas dépasser 100 caractères.")]
        public string Benchmark { get; set; }

        [StringLength(50, ErrorMessage = "Le nom du livre ne peut pas dépasser 50 caractères.")]
        public string Book { get; set; }

        [StringLength(100, ErrorMessage = "Le nom de création ne peut pas dépasser 100 caractères.")]
        public string CreationName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "La date de création doit être au format date.")]
        public DateTime? CreationDate { get; set; }

        [StringLength(100, ErrorMessage = "Le nom de révision ne peut pas dépasser 100 caractères.")]
        public string RevisionName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "La date de révision doit être au format date.")]
        public DateTime? RevisionDate { get; set; }

        [StringLength(100, ErrorMessage = "Le nom de l'affaire ne peut pas dépasser 100 caractères.")]
        public string DealName { get; set; }

        [StringLength(50, ErrorMessage = "Le type d'affaire ne peut pas dépasser 50 caractères.")]
        public string DealType { get; set; }

        [StringLength(50, ErrorMessage = "L'identifiant de la source ne peut pas dépasser 50 caractères.")]
        public string SourceListId { get; set; }

        [StringLength(50, ErrorMessage = "Le côté ne peut pas dépasser 50 caractères.")]
        public string Side { get; set; }
    }
}
