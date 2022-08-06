using Microsoft.AspNetCore.Mvc;

namespace EntityFramework.Controllers
{
    /**
     * ErrorController
     * Contains all the route bindings for the Error pages
     * ERROR
     * 
     * @author Reily Maahs
     * @student_number 040963994
     * @date 2022-08-06
     */
    public class ErrorController : Controller
    {
        /**
         * Error
         * Main view in the Error
         * 
         * @return Error view
         */
        public IActionResult Error()
        {
            return View();
        }
    }
}