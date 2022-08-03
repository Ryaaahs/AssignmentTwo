using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models
{
    public class Advertisement
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("File Name")]
        public string FileName { get; set; }

        [Required]
        [Url]
        [DisplayName(" Url")]
        public string Url { get; set; }
    }
}
