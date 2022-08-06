namespace EntityFramework.Models.ViewModels
{
    /**
     * FileInputViewModel
     * Used within Advertisement Create
     * Give access to the Brokerage that we're adding advertisements to
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class FileInputViewModel
    {
        public string BrokerageId { get; set; }
        public string BrokerageTitle { get; set; }
    }

}
