using Microsoft.AspNetCore.Identity;

namespace EventManagementUI.Models;

public class CustomIdentityUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Nationality { get; set; }
    public string Gender { get; set; }
    public string Location { get; set; }
    public bool Banned { get; set; }
}
