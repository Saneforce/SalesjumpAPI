namespace WebApplicationApi.Models
{
    public class getLists
    {
        public List<PriOrderHeader> headerDetails { get; set; }

        public List<PriOrdersRowDetails> rowDetails { get; set; }
    }
}
