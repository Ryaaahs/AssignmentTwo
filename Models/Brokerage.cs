using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    public class Brokerage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [DisplayName("Registration Number")]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [MinLength(3)]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayName("Fee")]
        public decimal Fee { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }

        public ICollection<Advertisement> Advertisements { get; set; }
    }
}
