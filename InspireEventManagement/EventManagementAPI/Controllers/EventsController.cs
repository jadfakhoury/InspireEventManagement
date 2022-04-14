using BookstopNetModels.Models;
using EventManagementAPI.Repositories;
using EventManagementLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Formatting;
using System.Text;

namespace EventManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admin")]
public class EventsController : ControllerBase
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
            throw;
        }
    }

    [HttpPost("PostNewEvent")]
    public async Task<IActionResult> PostNewEvent(Event newEvent)
    {
        int id;
        try
        {
            id = await _genericCRUD.Create<Event>(newEvent);
            return Ok(id);
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomepageAdmin\n" + error.ToString());
            throw;
        }
    }

    [HttpPost("UploadFileList")]
    public async Task<IActionResult> UploadFileList(List<IFormFile> files)
    {
        try
        {
            if (files == null || files.Count == 0)
                return NotFound();

            string[] fileDescription = files[0].FileName.Split('\\');
            string directory = fileDescription[0];

            foreach (var file in files)
            {
                fileDescription = file.FileName.Split('\\');
                string fileName = fileDescription[1];
                var pathMain = Path.Combine(Directory.GetCurrentDirectory(), @"..\Images\" + directory, fileName);




                using (var stream = new FileStream(pathMain, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomepageAdmin\n" + error.ToString());
            return BadRequest(new BadRequest(e.Message));
        }
    }
}
