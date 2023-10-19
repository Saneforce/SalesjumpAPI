namespace WebApplicationApi.Models
{
    public class TransOrderHead
    {

        public string? OrderTakenBy { get; set; }

        public string? DistributorCode { get; set; }
        public string? DocNumber { get; set; }
        public string? DocDate { get; set; }
        public string? OrderNo { get; set; }
        public string? OrderDate { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? StateName { get; set; }
        public string? OrderValue { get; set; }
        public string? GstinNo { get; set; }
        public string? Placeofsupply { get; set; }
        public string? TransType { get; set; }


        public TransOrderHead()
        {
            TransDetails = new List<TransOrderDetail>();
        }
        public List<TransOrderDetail> TransDetails { get; set; }
    }

    public class TransOrderDetail
    {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? UOM { get; set; }
        public string? ActualQty { get; set; }
        public string? BilledQty { get; set; }

        public string? CloseingStock { get; set; }
        public string? Rate { get; set; }
        public string? Amount { get; set; }
        public string? TaxCode { get; set; }
        public string? TaxPer { get; set; }
        public string? TaxAmount { get; set; }
    }
}
