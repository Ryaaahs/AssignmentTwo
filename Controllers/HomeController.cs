using Microsoft.AspNetCore.Mvc;

namespace EntityFramework.Controllers
{
    /**
     * HomeController
     * Contains all the route bindings for the Home pages
     * INDEX
     * ERROR
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class HomeController : Controller
    {
        /**
         * Index
         * Main view in the Home
         * 
         * @return Home Index page
         */
        public IActionResult Index()
        {
            return View();
        }

        /**
         * Error
         * Shared error page
         * 
         * @return Error page
         */
        public IActionResult Error()
        {
            return View();
        }

    }
}
