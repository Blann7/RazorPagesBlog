using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages.Auth
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        ApplicationContext context;
        public bool Success { get; set; } = false;
        public LogoutModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // ban check
            string? userId = HttpContext.User.FindFirst("Id")?.Value;

            User? user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            
            if(user is not null)
            {
                bool banned = AccessChecker.BanCheck(user.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            }
            // ---------

            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Success = true;
                return RedirectToPage("/index");
            }
            else
            {
                return Page();
            }
        }
    }
}
