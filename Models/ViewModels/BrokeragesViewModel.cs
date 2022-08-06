namespace EntityFramework.Models.ViewModels
{
    /**
     * BrokeragesViewModel
     * Used within Brokerage Index
     * Gives us the needed data to display the contents on the brokerage main page
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class BrokeragesViewModel
    {
        public IEnumerable<Brokerage> Brokerages { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
