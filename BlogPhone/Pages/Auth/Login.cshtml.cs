using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace BlogPhone.Pages.Auth
{
    public class LoginModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public string? UserName { get; set; }
        [BindProperty] public string? UserPassword { get; set; }
        [BindProperty] public bool UserSaveMe { get; set; } = false;
        public bool IsModalShow { get; set; } = false;
        public string Message { get; set; } = "";

        public LoginModel(ApplicationContext db)
        {
            context = db;
        }

        public void OnGet()
        { }

        public async Task<IActionResult> OnPostAsync()
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Name == UserName && u.Password == UserPassword);
            if (user is null) return Content("ѕользователь с такими данными не найден!", "text/html", Encoding.UTF8);

            List<Claim> claims = new List<Claim> {
                new Claim("Id", user.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            if (UserSaveMe)
            {
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });
            }
            else
            {
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(2) });
            }

            return RedirectToPage("/Index");
        }
    }
}
