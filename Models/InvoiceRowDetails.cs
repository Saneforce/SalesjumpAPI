using System.ComponentModel.DataAnnotations;

namespace WebApplicationApi.Models
{
    public class InvoiceRowDetails
    {
        [Key]
        public int ItemNo { get; set; }

        public string? ItemCode { get; set; }

        public string? ItemName { get; set; }

        public string? BatchNum { get; set; }

        public string? Quantity { get; set; }

        public string? Price { get; set; }

        public string? PackSize { get; set; }

        public string? UoM { get; set; }

        public string? TaxCode { get; set; }

        public string? TaxRate { get; set; }

    }
}
