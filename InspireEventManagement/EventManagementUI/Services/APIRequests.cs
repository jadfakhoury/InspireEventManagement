using EventManagementLibrary.Models;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;



namespace EventManagementUI.Services;

public class APIRequests
{
    private readonly ILogger<APIRequests> _logger;
    private readonly GlobalMethods publicMethods;
    internal IConfiguration Configuration { get; }

    public APIRequests(ILogger<APIRequests> logger, 
        GlobalMethods _publicMethods, IConfiguration configuration)
    {
        _logger = logger;
        publicMethods = _publicMethods;
        Configuration = configuration;
    }

    internal async Task<HttpResponseMessage> DELETEHttpRequest(string controllerStr, 
                                                                int id, Claim userClaim)
    {
        try
        {
            string tokenString = publicMethods.GenerateJSONWebToken(userClaim);
            using var httpClient = new HttpClient();
            publicMethods.InitiateHttpClient(tokenString, httpClient);
            return await httpClient.DeleteAsync(controllerStr + id);
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: PublicMethods\n" + error.ToString());
            throw;
        }
    }



    internal async Task<HttpResponseMessage> GETHttpRequest(string controllerStr, Claim userClaim)
    {
        try
        {
            string tokenString = publicMethods.GenerateJSONWebToken(userClaim);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", tokenString);

            httpClient.BaseAddress = new Uri(Configuration["WebApiUrl"]);
            return await httpClient.GetAsync(controllerStr);
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: PublicMethods\n" + error.ToString());
            throw;
        }
    }

    internal async Task<HttpResponseMessage> GETHttpRequest(string controllerStr, Claim userClaim, int id)
    {
        try
        {
            string tokenString = publicMethods.GenerateJSONWebToken(userClaim);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", tokenString);

            httpClient.BaseAddress = new Uri(Configuration["WebApiUrl"]);
            return await httpClient.GetAsync(controllerStr + id);
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: PublicMethods\n" + error.ToString());
            throw;
        }
    }

    internal async Task<HttpResponseMessage> POSTHttpRequest<T>(string controllerStr,
                                                                T content, Claim userClaim)
    {
        try
        {
            string tokenString = publicMethods.GenerateJSONWebToken(userClaim);
            using var httpClient = new HttpClient();
            publicMethods.InitiateHttpClient(tokenString, httpClient);
            return await httpClient.PostAsync(controllerStr, content, new JsonMediaTypeFormatter());
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: PublicMethods\n" + error.ToString());
            throw;
        }
    }

    internal async Task<HttpResponseMessage> POSTUploadHttpRequest(string controllerStr,
                                                                    List<IFormFile> files, 
                                                                    string directory, Claim userClaim)
    {
        try
        {
            string guid = "";

            string tokenString = publicMethods.GenerateJSONWebToken(userClaim);
            HttpResponseMessage response = new HttpResponseMessage();
            using (var httpClient2 = new HttpClient())
            {
                publicMethods.InitiateHttpClient(tokenString, httpClient2);
                MultipartFormDataContent content;

                if (files.Count > 0)
                {
                    using (content = new MultipartFormDataContent())
                    {
                        string fileName = "";
                        foreach (var file in files)
                        {
                            guid = Guid.NewGuid().ToString();
                            fileName = guid + Path.GetExtension(file.FileName);
                            content.Add(new StreamContent(file.OpenReadStream())
                            {
                                Headers =
                            {
                                ContentLength = file.Length,
                                ContentType = new MediaTypeHeaderValue(file.ContentType)
                            }
                            }, "files", directory + '\\' + fileName);

                        }

                        response = await httpClient2.PostAsync(controllerStr, content);
                    }
                }
                else
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
            return response;
        }
        catch (Exception e)
        {
            ResponseModel error = new ResponseModel(e);
            _logger.LogError("\nSource: PublicMethods\n" + error.ToString());
            throw;
        }
    }
}
