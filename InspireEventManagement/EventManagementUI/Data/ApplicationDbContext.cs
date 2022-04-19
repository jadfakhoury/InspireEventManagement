using EventManagementUI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventManagementUI.Data;

public class ApplicationDbContext : IdentityDbContext<CustomIdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    public DbSet<EventManagementLibrary.Models.Event> Event { get; set; }
}
