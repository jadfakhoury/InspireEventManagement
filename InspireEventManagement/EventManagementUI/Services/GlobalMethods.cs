using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace EventManagementUI.Services;

public class GlobalMethods
{
    internal IConfiguration Configuration { get; }
    public GlobalMethods(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    internal string GenerateJSONWebToken(Claim userClaim)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

        SigningCredentials credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);
        Claim[] claim = new[] { userClaim };

        var token = new JwtSecurityToken(
            issuer: Configuration["Jwt:Issuer"],
            audience: Configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials,
            claims: claim
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    internal void InitiateHttpClient(string tokenString, HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", tokenString);

        httpClient.DefaultRequestHeaders.Accept.Clear();

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.BaseAddress = new Uri(Configuration["WebApiUrl"]);
    }

    internal Claim GetUserClaim(ClaimsPrincipal user)
    {
        if (user.IsInRole("Admin"))
            return new Claim("Role", "Admin");
        else if (user.IsInRole("User"))
            return new Claim("Role", "User");
        else return new Claim("Role", "Unauthorized");
    }

    internal async Task<T> Deserialize<T>(HttpResponseMessage responseMessage) where T : class
    {
        string apiResponse = await responseMessage.Content.ReadAsStringAsync();
        T responseObject = JsonConvert.DeserializeObject<T>(apiResponse);
        return responseObject;
    }

    internal async Task<List<T>> DeserializeList<T>(HttpResponseMessage responseMessage) where T : class
    {
        string apiResponse = await responseMessage.Content.ReadAsStringAsync();
        List<T> responseList = JsonConvert.DeserializeObject<List<T>>(apiResponse);
        return responseList;
    }
}
