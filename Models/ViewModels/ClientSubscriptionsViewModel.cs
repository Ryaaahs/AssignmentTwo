namespace EntityFramework.Models.ViewModels
{
    /**
     * ClientsBrokerageView
     * Used within Client EditSubscriptions
     * Stores the client that we're modifiying and the list of subscriptions they are/not a member.
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class ClientSubscriptionsViewModel
    {
        public Client Client { get; set; }
        public IList<BrokerageSubscriptionsViewModel> Subscriptions { get; set; }

    }
}
