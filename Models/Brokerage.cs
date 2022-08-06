using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    /**
     * Brokerage
     * Model class of the Brokerage Entity
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
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
