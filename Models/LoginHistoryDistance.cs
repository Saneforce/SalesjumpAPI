namespace WebApplicationApi.Models
{
    public class DistanceMatrixDetails
    {
        public string sfcode { get;set; }
        public string departureTime { get; set; }

        public List<Originslatlong> origins { get; set; }

        public List<Distinationslatlong> destinations { get; set; }

        //public string[] origins { get; set; }

        //public string destinations { get; set; }

        public string mode  { get; set; }

    }

    public class Originslatlong
    {
        public double latitude { get; set; }

        public double longitude { get; set; }
    }

    public class Distinationslatlong
    {
        public double latitude { get; set; }

        public double longitude { get; set; }
    }

    
}
