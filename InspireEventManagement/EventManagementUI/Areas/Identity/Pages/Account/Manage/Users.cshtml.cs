using EventManagementUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic;
using System.Security.Claims;

namespace EventManagementUI.Areas.Identity.Pages.Account.Manage;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly UserManager<CustomIdentityUser> _userManager;

    public UsersModel(UserManager<CustomIdentityUser> userManager)
    {
        _userManager = userManager;
    }



    [BindProperty]
    public DataTablesRequest DataTablesRequest { get; set; }

    public void OnGet()
    {

    }

    public async Task<JsonResult> OnPostGetUsers()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var recordsTotal = _userManager.Users.Where(u => u.Id != currentUserId).Count();
        var customersQuery = _userManager.Users.Where(u => u.Id != currentUserId).AsQueryable();

        var searchText = DataTablesRequest.Search.Value?.ToUpper();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            customersQuery = customersQuery.Where(s =>
                s.FirstName.ToUpper().Contains(searchText) ||
                s.LastName.ToUpper().Contains(searchText) ||
                s.Gender.ToUpper().Contains(searchText) ||
                s.Nationality.ToUpper().Contains(searchText)
            );
        }

        var recordsFiltered = customersQuery.Count();

        var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
        var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

        customersQuery = customersQuery.OrderBy($"{sortColumnName} {sortDirection}");

        var skip = DataTablesRequest.Start;
        var take = DataTablesRequest.Length;
        var data = await customersQuery
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new JsonResult(new
        {
            Draw = DataTablesRequest.Draw,
            RecordsTotal = recordsTotal,
            RecordsFiltered = recordsFiltered,
            Data = data
        });
    }

    public async Task<JsonResult> OnPostBanUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user.Banned)
            user.Banned = false;
        else
            user.Banned = true;

        await _userManager.UpdateAsync(user);
        return new JsonResult(200);
    }
}


