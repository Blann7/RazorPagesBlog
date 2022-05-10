using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using BlogPhone.Models.Auth;

namespace BlogPhone.Pages
{
    public class RegisterModel : PageModel
    {
        ApplicationContext context;
        [BindProperty] public User SiteUser { get; set; } = new();

        public RegisterModel(ApplicationContext db)
        {
            context = db;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string confirmPassword = Request.Form["passwordagain"];
            if (confirmPassword is null || SiteUser.Password != confirmPassword)
            {
                return Page();
            }

            await context.Users.AddAsync(SiteUser);
            await context.SaveChangesAsync();

            RegisteredOptions registeredOptions = new RegisteredOptions();
            return Redirect($"Registered/{registeredOptions.regKey}");
        }
    }
}
