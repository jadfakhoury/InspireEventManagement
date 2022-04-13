using Microsoft.AspNetCore.Mvc;

namespace EventManagementUI.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
