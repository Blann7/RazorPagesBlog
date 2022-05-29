using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace BlogPhone.Pages
{
    public class RegisterModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public User SiteUser { get; set; } = new();

        public RegisterModel(ApplicationContext db)
        { 
            context = db; 
        }
        public async Task<IActionResult> OnPostAsync()
        {
            string confirmPassword = Request.Form["passwordagain"];
            if (confirmPassword is null || SiteUser.Password != confirmPassword)
            {
                return Page();
            }

            // To avoid repetition
            User? user = await context.Users
                .Select(u => new User { Name = u.Name })
                .FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if (user is not null) return Content("ѕользователь с таким именем уже есть!", "text/html", Encoding.UTF8);

            user = await context.Users
                .Select(u => new User { Email = u.Email })
                .FirstOrDefaultAsync(u => u.Email == SiteUser.Email);
            if (user is not null) return Content("ѕользователь с такой почтой уже есть!", "text/html", Encoding.UTF8);
            //--------------------

            SiteUser.Role = "user"; // Default Role for all site's users

            await context.Users.AddAsync(SiteUser);
            await context.SaveChangesAsync();

            // login --------------------------------------
            User? loginUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Name = u.Name })
                .FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if(loginUser is null) return RedirectToPage("/auth/login");

            List<Claim> claims = new() {
                new Claim("Id", loginUser.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new (claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties authenticationProperties = new() { AllowRefresh = true }; // expire: session

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties);
            // --------------------------------------------

            return RedirectToPage("/index");
        }
    }
}
