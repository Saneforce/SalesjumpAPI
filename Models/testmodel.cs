using System.ComponentModel.DataAnnotations;
namespace WebApplicationApi.Models
{
    public class Testmodel
    {
        [Key]
       
        public string? Fieldcode { get; set; }

        public string? Filedname { get; set; }

        public string? state { get; set; }

        public string? Insertdate { get; set; }
    }
}
