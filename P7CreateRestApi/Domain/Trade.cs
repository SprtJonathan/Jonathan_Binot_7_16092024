using System;
using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class Trade
    {
        [Required(ErrorMessage = "L'identifiant du trade est requis.")]
        public int TradeId { get; set; }

        [Required(ErrorMessage = "Le compte est requis.")]
        [StringLength(50, ErrorMessage = "Le compte ne peut pas d�passer 50 caract�res.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Le type de compte est requis.")]
        [StringLength(50, ErrorMessage = "Le type de compte ne peut pas d�passer 50 caract�res.")]
        public string AccountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantit� achet�e doit �tre un nombre positif.")]
        public double? BuyQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantit� vendue doit �tre un nombre positif.")]
        public double? SellQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix d'achat doit �tre un nombre positif.")]
        public double? BuyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix de vente doit �tre un nombre positif.")]
        public double? SellPrice { get; set; }

        [DataType(DataType.Date, ErrorMessage = "La date de trade doit �tre au format date.")]
        public DateTime? TradeDate { get; set; }

        [StringLength(50, ErrorMessage = "La s�curit� du trade ne peut pas d�passer 50 caract�res.")]
        public string TradeSecurity { get; set; }

        [StringLength(50, ErrorMessage = "Le statut du trade ne peut pas d�passer 50 caract�res.")]
        public string TradeStatus { get; set; }

        [StringLength(50, ErrorMessage = "Le nom du trader ne peut pas d�passer 50 caract�res.")]
        public string Trader { get; set; }

        [StringLength(100, ErrorMessage = "Le benchmark ne peut pas d�passer 100 caract�res.")]
        public string Benchmark { get; set; }

        [StringLength(50, ErrorMessage = "Le nom du livre ne peut pas d�passer 50 caract�res.")]
        public string Book { get; set; }

        [StringLength(100, ErrorMessage = "Le nom de cr�ation ne peut pas d�passer 100 caract�res.")]
        public string CreationName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "La date de cr�ation doit �tre au format date.")]
        public DateTime? CreationDate { get; set; }

        [StringLength(100, ErrorMessage = "Le nom de r�vision ne peut pas d�passer 100 caract�res.")]
        public string RevisionName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "La date de r�vision doit �tre au format date.")]
        public DateTime? RevisionDate { get; set; }

        [StringLength(100, ErrorMessage = "Le nom de l'affaire ne peut pas d�passer 100 caract�res.")]
        public string DealName { get; set; }

        [StringLength(50, ErrorMessage = "Le type d'affaire ne peut pas d�passer 50 caract�res.")]
        public string DealType { get; set; }

        [StringLength(50, ErrorMessage = "L'identifiant de la source ne peut pas d�passer 50 caract�res.")]
        public string SourceListId { get; set; }

        [StringLength(50, ErrorMessage = "Le c�t� ne peut pas d�passer 50 caract�res.")]
        public string Side { get; set; }
    }
}
