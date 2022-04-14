using BookstopNetModels.Models;
using EventManagementAPI.ErrorHandling;
using EventManagementAPI.Repositories;
using EventManagementLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admin")]
public class EventsController : Controller
{
    private readonly ILogger<EventsController> _logger;
    private readonly GenericCRUD _genericCRUD;

    public EventsController(ILogger<EventsController> logger, GenericCRUD genericCRUD)
    {
        _logger = logger;
        _genericCRUD = genericCRUD; 
    }

    [HttpGet("GetEventsList")]
    public IActionResult EventsList()
    {
        try
        {
            List<Event> events = _genericCRUD.GetList<Event>();
            return Ok(events);
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomepageAdmin\n" + error.ToString());
            return NotFound(new NotFoundError(e.Message));
        }
    }
}
