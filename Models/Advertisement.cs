using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    /**
     * Advertisement
     * Model class of the Advertisement Entity
     * This is used to store Brokerage Advertisements we create in the application
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
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

        [Required]
        public string BrokerageId { get; set; }

        [ForeignKey("BrokerageId")]
        public Brokerage Brokerage { get; set; }
    }
}
