using EventManagementAPI.Repositories;
using EventManagementLibrary.DBContext;
using EventManagementLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ILogger<EventsController> _logger;
    private readonly GenericCRUD<EventDBContext> _genericCRUD;

    public EventsController(ILogger<EventsController> logger, GenericCRUD<EventDBContext> genericCRUD)
    {
        _logger = logger;
        _genericCRUD = genericCRUD;
    }

    [HttpGet("GetEventsList")]
    [Authorize(Policy = "User")]
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
            return BadRequest(error);
        }
    }

    [HttpPost("PostNewEvent")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> PostNewEvent(Event newEvent)
    {
        int id;
        try
        {
            id = await _genericCRUD.Create<Event>(newEvent);
            newEvent.Images = id.ToString();
            await _genericCRUD.Edit<Event>(newEvent);
            return Ok(id);
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomepageAdmin\n" + error.ToString());
            return BadRequest(error);
        }
    }

    [HttpPost("UploadFileList")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> UploadFileList(List<IFormFile> files)
    {
        try
        {
            if (files == null || files.Count == 0)
                return NotFound();

            string[] fileDescription = files[0].FileName.Split('\\');
            string directoryPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).ToString(), 
                                    @"EventManagementUI\wwwroot\Images\" + fileDescription[0]);
            if(!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            foreach (var file in files)
            {
                fileDescription = file.FileName.Split('\\');
                string fileName = fileDescription[1];
                var pathMain = Path.Combine(directoryPath, fileName);

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
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetEventById/{id}")]
    [Authorize(Policy = "User")]
    public IActionResult GetEventById(int id)
    {
        try
        {
            Event eventDetails = _genericCRUD.GetbyId<Event>(id);
            return Ok(eventDetails);
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomepageAdmin\n" + error.ToString());
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("DeleteEvent/{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        try
        {
            await _genericCRUD.Delete<Event>(id);
            string dirName = id.ToString();
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).ToString(), 
                                @"EventManagementUI\wwwroot\Images\" + dirName);
            System.IO.Directory.Delete(path,true);
            return Ok();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomepageAdmin\n" + error.ToString());
            return BadRequest(e.Message);
        }
    }

    [HttpPost("EditEvent")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> EditEvent(Event editEvent)
    {
        try
        {
            await _genericCRUD.Edit(editEvent);
            return Ok();
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomepageAdmin\n" + error.ToString());
            return BadRequest(e.Message);
        }
    }
}
