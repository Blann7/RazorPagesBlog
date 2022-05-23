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

        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Name == UserName && u.Password == UserPassword);
            if (user is null) return Content("ѕользователь с такими данными не найден!", "text/html", Encoding.UTF8);
            if (string.IsNullOrEmpty(user.Role)) return BadRequest();

            List<Claim> claims = new List<Claim> {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authenticationProperties = new();
            if (UserSaveMe)
                authenticationProperties = new AuthenticationProperties { IsPersistent = true, AllowRefresh = true };
            else
                authenticationProperties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10), 
                                                                            AllowRefresh = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties);

            return RedirectToPage(returnUrl ?? "/index");
        }
    }
}
