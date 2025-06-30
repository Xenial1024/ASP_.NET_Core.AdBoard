using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/Exception")]
        public IActionResult Exception() => View();

        [Route("Error/NotFound")]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404; 
            return View("NotFound"); 
        }
    }
}