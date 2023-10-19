using System.ComponentModel.DataAnnotations;

namespace WebApplicationApi.Models
{
    public class PriOrdersHeaderDetails
    {
        [Key]
        public int CustomerNo { get; set; }
        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string Series { get; set; }

        public string SeriesName { get; set; }

        public string InvoiceNum { get; set; }

        public string InvoiceDate { get; set; }

        public string SaleEmpCode { get; set; }

        public string StateCode { get; set; }

        public string GSTIN { get; set; }

        public List<PriOrdersRowDetails> rowDetails { get; set; }
    }
}
