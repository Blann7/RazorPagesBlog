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
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            string confirmPassword = Request.Form["passwordagain"];
            if (confirmPassword is null || SiteUser.Password != confirmPassword)
            {
                return Page();
            }

            // to avoid repetition
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if (user is not null) return Content("ѕользователь с таким именем уже есть!", "text/html", Encoding.UTF8);

            user = await context.Users.FirstOrDefaultAsync(u => u.Email == SiteUser.Email);
            if (user is not null) return Content("ѕользователь с такой почтой уже есть!", "text/html", Encoding.UTF8);
            //--------------------

            SiteUser.Role = "user"; // default Role for all site's users

            await context.Users.AddAsync(SiteUser);
            await context.SaveChangesAsync();

            // login --------------------------------------
            User? loginUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if(loginUser is null) return RedirectToPage("/auth/login");

            List<Claim> claims = new() {
                new Claim("Id", loginUser.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, loginUser.Role?.ToString() ?? "undefined")
            };

            ClaimsIdentity claimsIdentity = new (claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5), AllowRefresh = true });
            // --------------------------------------------

            return RedirectToPage("/index");
        }
    }
}
