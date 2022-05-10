using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.HttpSys;

namespace BlogPhone.Pages.Auth
{
    public class LoginModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public string? UserName { get; set; }
        [BindProperty] public string? UserPassword { get; set; }
        [BindProperty] public bool UserSaveMe { get; set; } = false;
        public List<User> SiteUsers { get; set; } = new();

        public LoginModel(ApplicationContext db)
        {
            context = db;
        }

        public async Task OnGetAsync()
        {
            SiteUsers = await context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Name == UserName && u.Password == UserPassword);
            if (user is null) return Content("Пользователь с такими данными не найден!", "text/html", Encoding.UTF8);
            // u => u.Name == UserName && u.Password == UserPassword

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
                new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1) });
            }

            return Content("Вы успешно вошли", "text/html", Encoding.UTF8);
        }
    }
}
