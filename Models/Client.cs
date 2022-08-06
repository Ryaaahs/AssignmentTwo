using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    /**
     * Client
     * Model class of the Client Entity
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]

        public DateTime BirthDate { get; set; }

        public string FullName { get { return FirstName + ", " + LastName; } }

        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
