namespace WebApplicationApi.Models
{
    public class IODetails
    {
        public int CustomerNo { get; set; }

        public string? CustomerCode { get; set; }

        public string? CustomerName { get; set; }

        public string? Series { get; set; }

        public string? SeriesName { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string? InvoiceNum { get; set; }

        public string? SaleEmpCode { get; set; }

        public string? StateCode { get; set; }

        public string? GSTIN { get; set; }

        public string? ItemCode { get; set; }

        public string? ItemName { get; set; }

        public string? BatchNum { get; set; }

        public Decimal Quantity { get; set; }

        public Decimal Price { get; set; }

        public string? PackSize { get; set; }

        public string? UoM { get; set; }

        public string? TaxCode { get; set; }

        public Decimal TaxRate { get; set; }

    }
}
