using EventManagementLibrary.Models;
using EventManagementUI.Models;
using EventManagementUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagementUI.Controllers;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly GlobalMethods _globalMethods;
    public IConfiguration _configuration;
    private readonly APIRequests _apiRequests;
    private readonly GlobalConfig _globalConfig;


    public UserController(ILogger<AdminController> logger, GlobalMethods globalMethods,
        APIRequests apiRequests, IConfiguration configuration, GlobalConfig globalConfig)
    {
        _logger = logger;
        _globalMethods = globalMethods;
        _configuration = configuration;
        _apiRequests = apiRequests;
        _globalConfig = globalConfig;
    }

    [HttpGet]
    public IActionResult EventsList()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EventsTablePartial()
    {
        try
        {
            Claim userClaim = _globalMethods.GetUserClaim(User);
            HttpResponseMessage response = await _apiRequests.GETHttpRequest(_configuration["APIController"] + "/GetEventsList", userClaim);
            if (response.IsSuccessStatusCode)
            {
                List<Event> majorServices = new List<Event>();
                majorServices = await _globalMethods.DeserializeList<Event>(response);
                return PartialView(majorServices);
            }
            else
            {
                ResponseModel responseObject = await _globalMethods.Deserialize<ResponseModel>(response);
                _logger.LogError(responseObject.ToString());
                return View("Error", responseObject);
            }
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomeController\n" + error.ToString());
            return View("Error", error);
        }
    }

    [HttpGet]
    public async Task<ActionResult> EventDetails(int id)
    {

        try
        {
            Claim userClaim = _globalMethods.GetUserClaim(User);
            HttpResponseMessage response = await _apiRequests.GETHttpRequest(_configuration["APIController"] + "/GetEventById/", userClaim, id);
            if (response.IsSuccessStatusCode)
            {
                Event eventObj = await _globalMethods.Deserialize<Event>(response);
                EventViewModel eventDetails = new EventViewModel { Event = eventObj };
                var imgPath = "./wwwroot/Images/" + eventObj.Images.Trim();
                ViewData["ImagesList"] = GetFilesFrom(imgPath, _globalConfig.ImagesFilter()).Select(i => Path.GetFileName(i)).ToList();
                return View(eventDetails);
            }
            else
            {
                ResponseModel responseObject = await _globalMethods.Deserialize<ResponseModel>(response);
                _logger.LogError(responseObject.ToString());
                return View("Error", responseObject);
            }
        }
        catch (Exception)
        {
            ResponseModel error = new ResponseModel(
                new Exception("Something went wrong and the model state was not Valid!"));
            _logger.LogError("\nSource: AdminController\n" + error.ToString());

            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public IActionResult EventsCalendar()
    {
        return View();
    }

    private List<string> GetFilesFrom(String searchFolder, String[] filters)
    {
        List<String> filesFound = new List<String>();
        var searchOption = SearchOption.TopDirectoryOnly;
        foreach (var filter in filters)
        {
            filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
        }
        return filesFound.ToList();
    }
}
