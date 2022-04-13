using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Events()
    {
        return View();
    }

    //[HttpGet]
    //public async Task<IActionResult> EventsTablePartial()
    //{

    //}

    //[HttpGet]
    //public IActionResult NewEvent()
    //{
    //    return View();
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> NewEvent()
    //{

    //}

    //[HttpGet]
    //public async Task<ActionResult> EditEvent(int id)
    //{

    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> EditEvent()
    //{

    //}

    //[HttpGet]
    //public async Task<IActionResult> DeleteEvent(int id)
    //{

    //}
}