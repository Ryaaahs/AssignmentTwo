using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    /**
     * Subscription
     * Model class of the Subscription Entity
     * Bridge Entity between the Clients and Brokerages, allowing them to interact
     * by letting Clients register/unregister to them.
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class Subscription
    {
        public int Id { get; set; }
        
        [Required]
        public int ClientId { get; set; }

        [Required]
        public string BrokerageId { get; set; }

        [ForeignKey("BrokerageId")]
        public Brokerage Brokerage { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }
    }
}
