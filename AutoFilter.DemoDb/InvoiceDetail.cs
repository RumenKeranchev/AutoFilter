using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AutoFilter.DemoDb
{
    public class InvoiceDetail : Entity
    {
        [ForeignKey(nameof(Invoice))]
        public int Invoice_Id { get; set; }
        [JsonIgnore]
        public Invoice? Invoice { get; set; }

        public required string Goods { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }
    }
}
