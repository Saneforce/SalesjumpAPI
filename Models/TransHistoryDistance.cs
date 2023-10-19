
namespace WebApplicationApi.Models
{
    public class TransHistoryDistance
    {
        public string Sf_Code { get; set; }

        public int ListedDrCode { get; set; }

        public string ListedDrName { get; set; }
               
        public string ModTime { get; set; } 

        public double TLati { get; set; }

        public double TLong { get; set; }

        public double POBValue { get; set; }        
    }

}
