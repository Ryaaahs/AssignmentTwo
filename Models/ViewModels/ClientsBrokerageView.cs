namespace EntityFramework.Models.ViewModels
{
    /**
     * ClientsBrokerageView
     * Used within Client Index
     * Gives us the needed data to display the contents on the client main page
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class ClientsBrokerageView
    {
        public IEnumerable<String> CurrentClientBrokerages { get; set; }
        public IEnumerable<Client> Clients { get; set; }
    }
}
