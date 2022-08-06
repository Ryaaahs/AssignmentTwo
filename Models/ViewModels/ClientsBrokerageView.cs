namespace EntityFramework.Models.ViewModels
{
    public class ClientsBrokerageView
    {
        public IEnumerable<String> CurrentClientBrokerages { get; set; }
        public IEnumerable<Client> Clients { get; set; }
    }
}
