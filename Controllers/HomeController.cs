using Microsoft.AspNetCore.Mvc;

namespace OnlineRentalSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
