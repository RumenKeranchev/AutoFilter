using System.ComponentModel.DataAnnotations;

namespace AutoFilter.DemoDb
{
    public class Invoice : Entity
    {
        [StringLength(20), Required]
        public string Number { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public DateTime? SentDate { get; set; }

        public decimal Total { get; set; }

        public bool IsPaid { get; set; }
    }
}
