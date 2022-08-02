namespace EntityFramework.Models.ViewModels
{
    public class BrokeragesViewModel
    {
        public IEnumerable<Brokerage> Brokerages { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
