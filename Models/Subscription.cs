using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
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
