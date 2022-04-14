using EventManagementAPI.Repositories;
using EventManagementLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly GenericCRUD _genericCRUD;

    public AdminController(ILogger<AdminController> logger, GenericCRUD genericCRUD)
    {
        _logger = logger;
        _genericCRUD = genericCRUD;
    }
    //[HttpGet("GetEventsList")]
    //public IActionResult EventsList()
    //{
    //    List<Event> events = _genericCRUD.GetList<Event>();
    //    return View();
    //}

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