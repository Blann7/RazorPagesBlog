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
        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            User? user = await context.Users
                .Select(u => new User { Id = u.Id, Name = u.Name, Password = u.Password })
                .FirstOrDefaultAsync(u => u.Name == UserName && u.Password == UserPassword);
            if (user is null || user.Name != UserName || user.Password != UserPassword) // to register observation
                return Content("ѕользователь с такими данными не найден!", "text/html", Encoding.UTF8);

            List<Claim> claims = new ()
            {
                new Claim("Id", user.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new (claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authenticationProperties = new();
            if (UserSaveMe)
                authenticationProperties = new () { 
                    IsPersistent = true, 
                    AllowRefresh = true, 
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30) 
                };
            else
                authenticationProperties = new () 
                { 
                    AllowRefresh = true // expire: session
                };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties);

            return RedirectToPage(returnUrl ?? "/index");
        }
    }
}
