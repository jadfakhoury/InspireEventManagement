
using EventManagementLibrary.Models;
using EventManagementUI.Models;
using EventManagementUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace EventManagementUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly GlobalMethods _globalMethods;
    public IConfiguration _configuration;
    private readonly APIRequests _apiRequests;
    private readonly GlobalConfig _globalConfig;


    public AdminController(ILogger<AdminController> logger, GlobalMethods globalMethods,
        APIRequests apiRequests, IConfiguration configuration, GlobalConfig globalConfig)
    {
        _logger = logger;
        _globalMethods = globalMethods;
        _configuration = configuration;
        _apiRequests = apiRequests;
        _globalConfig = globalConfig;
    }

    [HttpGet]
    public IActionResult EventsList(string alert = null)
    {
        if (alert != null)
            ViewData["alert"] = alert;

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
    public IActionResult NewEvent()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewEvent(EventViewModel eventModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                Claim userClaim = _globalMethods.GetUserClaim(User);
                Event newEvent = eventModel.Event;

                HttpResponseMessage response = await _apiRequests.POSTHttpRequest<Event>(_configuration["APIController"] + "/PostNewEvent", newEvent, userClaim);
                HttpResponseMessage uploadSuccess = new HttpResponseMessage();
                string folderId = "";
                if (response.IsSuccessStatusCode)
                {
                    folderId = await response.Content.ReadAsStringAsync();
                    if (eventModel.detailsImageList != null)
                    {
                        uploadSuccess = await _apiRequests.POSTUploadHttpRequest(_configuration["APIController"] + "/UploadFileList", eventModel.detailsImageList, folderId, userClaim);

                        if (uploadSuccess.IsSuccessStatusCode)
                            newEvent.Images = folderId;
                    }

                    return RedirectToAction("EventsList", new { alert = "Event Created Successfully" });
                }
                else
                {
                    ResponseModel responseObject = await _globalMethods.Deserialize<ResponseModel>(response);
                    _logger.LogError(message: "\nSource: HomeController\n" + responseObject.ToString());
                    return Content(JsonConvert.SerializeObject(responseObject));
                }

            }
            catch (Exception e)
            {
                ResponseModel error = new ResponseModel(e);
                _logger.LogError("\nSource: HomeController\n" + error.ToString());
                return View("Error", error);
            }
        }
        else
        {
            ResponseModel error = new ResponseModel
            (
                new Exception("Something went wrong and the model state was not Valid!")
            );
            _logger.LogError("\nSource: AdminController\n" + error.ToString());
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
                if(Directory.Exists(imgPath))
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

    [HttpGet]
    public async Task<ActionResult> EditEvent(int id)
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
                if (Directory.Exists(imgPath))
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
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home", new { ErrorTitle = "Database Error", ErrorMessage = ex.Message.ToString() });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditEvent(EventViewModel eventViewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                Claim userClaim = _globalMethods.GetUserClaim(User);
                Event toEdit = eventViewModel.Event;
                HttpResponseMessage response = await _apiRequests.POSTHttpRequest<Event>(_configuration["APIController"] + "/EditEvent", toEdit, userClaim);
                HttpResponseMessage uploadSuccess = new HttpResponseMessage();

                if (response.IsSuccessStatusCode)
                {
                    if(eventViewModel.detailsImageList != null)
                        uploadSuccess = await _apiRequests.POSTUploadHttpRequest(_configuration["APIController"] + "/UploadFileList", eventViewModel.detailsImageList, eventViewModel.Event.Images.Trim(), userClaim);
                }
                return RedirectToAction("EventsList", new { alert = "Event Edited Successfully" });
            }
            catch (Exception)
            {

                ResponseModel error = new ResponseModel(
                    new Exception("Something went wrong and the model state was not Valid!"));
                _logger.LogError("\nSource: AdminController\n" + error.ToString());

                return RedirectToAction("Error", "Home");
            }
        }
        else
        {
            return RedirectToAction("EditEvent", new { id = eventViewModel.Event.Id });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        try
        {
            Claim userClaim = _globalMethods.GetUserClaim(User);

            if (id > -1)
            {
                HttpResponseMessage response = await _apiRequests.DELETEHttpRequest(_configuration["APIController"] + "/DeleteEvent/", id, userClaim);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("EventsList");
                }
                else
                {
                    ResponseModel responseObject = await _globalMethods.Deserialize<ResponseModel>(response);
                    _logger.LogError("\nSource: HomeController\n" + responseObject.ToString());
                    return Content(JsonConvert.SerializeObject(responseObject));
                }
            }
            else
            {
                ResponseModel error = new ResponseModel
                (
                        new Exception("Something went wrong and the model state was not Valid!")
                    );
                _logger.LogError("\nSource: AdminController\n" + error.ToString());

                return Content(JsonConvert.SerializeObject(error));
            }
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: HomeController\n" + error.ToString());
            return View("Error", error);
        }

    }
}