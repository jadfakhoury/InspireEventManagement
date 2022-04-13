// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EventManagementUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementUI.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<CustomIdentityUser> _userManager;

        public IndexModel(UserManager<CustomIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string Email { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Location")]
            public string Location { get; set; }

            [Required]
            [Display(Name = "Nationality")]
            public string Nationality { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }
        }

        public List<string> Nationalities { get; set; }
        public List<string> Genders { get; set; }


        private async Task LoadAsync(CustomIdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;
            Nationality = user.Nationality;
            Gender = user.Gender;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender =  user.Gender,
                Nationality = user.Nationality,
                Location = user.Location
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            Nationalities = GetNationalities();
            Genders = GetGenders();

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Gender = Input.Gender.Trim();
            user.Nationality = Input.Nationality.Trim();
            user.Location = Input.Location;
            await _userManager.UpdateAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }


        private List<string> GetGenders()
        {
            return new List<string> { "Male", "Female", "Other" };
        }
        private List<string> GetNationalities()
        {
            return new List<string> { "Lebanese", "German", "Italian", "French" };
        }
    }
}
