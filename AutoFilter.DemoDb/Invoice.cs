using System.ComponentModel.DataAnnotations;

namespace AutoFilter.DemoDb
{
    public class Invoice : Entity
    {
        [StringLength(20), Required]
        public required string Number { get; set; }
                        
        public required string Type { get; set; }

        public required string Status { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? SentDate { get; set; }

        public decimal VatBase { get; set; }

        public decimal Vat { get; set; }

        public decimal Total { get; set; }

        public ICollection<InvoiceDetail>? Details { get; set; }
    }
}
