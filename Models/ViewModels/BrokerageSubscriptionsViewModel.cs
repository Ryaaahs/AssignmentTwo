namespace EntityFramework.Models.ViewModels
{
    /**
     * BrokerageSubscriptionsViewModel
     * Used within Client EditSubscription
     * This allows us to confirm if a Client is subscibed to a brokerage
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class BrokerageSubscriptionsViewModel
    {
            public string BrokerageId { get; set; }
            public string Title { get; set; }
            public bool IsMember { get; set; }
        
    }
}
