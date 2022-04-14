
using BookstopNetModels.Models;
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


    public AdminController(ILogger<AdminController> logger, GlobalMethods globalMethods, APIRequests apiRequests, IConfiguration configuration)
    {
        _logger = logger;
        _globalMethods = globalMethods;
        _configuration = configuration;
        _apiRequests = apiRequests;
    }

    [HttpGet("GetEventsList")]
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
                ViewBag.VPath = _configuration["VirtualDir"];
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

                if (response.IsSuccessStatusCode)
                {
                    var folderId = await response.Content.ReadAsStringAsync();
                    if (eventModel.detailsImageList != null)
                    {
                        ImageListUploadViewModel imageList = new ImageListUploadViewModel();

                        imageList = await _apiRequests.POSTUploadHttpRequest(_configuration["APIController"] + "/ UploadFileList", eventModel.detailsImageList, folderId, userClaim);

                        if (imageList.Response.IsSuccessStatusCode)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var item in imageList.ImagesNames)
                            {
                                sb.Append(item + ';');
                            }
                            newEvent.Images += sb.ToString();
                        }
                        else
                        {
                            HttpResponseMessage requestResponse = await _globalMethods.Deserialize<HttpResponseMessage>(imageList.Response);
                            _logger.LogError("\nSource: HomeController\n" + requestResponse.ToString());
                            return Content(JsonConvert.SerializeObject(requestResponse));
                        }
                    }
                    else
                        newEvent.Images = eventModel.Event.Images;

                    ResponseModel responseObject = new ResponseModel
                    (
                        new Exception("Data saved Successfully.")
                    );
                    return Content(JsonConvert.SerializeObject(responseObject));
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