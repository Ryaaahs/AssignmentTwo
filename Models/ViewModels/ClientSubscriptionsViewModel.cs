namespace EntityFramework.Models.ViewModels
{
    public class ClientSubscriptionsViewModel
    {
        public Client Client { get; set; }
        public IList<BrokerageSubscriptionsViewModel> Subscriptions { get; set; }

    }
}
