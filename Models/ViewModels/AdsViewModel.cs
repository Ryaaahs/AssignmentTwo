using EntityFramework.Models;

namespace EntityFramework.Models.ViewModels
{
    /**
     * AdsViewModel
     * Used within the Advertisement Index page 
     * As we need a list of adverisements that are assoicated with a brokerage
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class AdsViewModel
    {
        public Brokerage Brokerage { get; set; }
        public IEnumerable<Advertisement> Advertisements { get; set; }
    }
}
