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
        readonly ApplicationContext context;
        public User? SiteUser { get; set; }
        public LogoutModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            // ban check
            bool banned = AccessChecker.BanCheck(SiteUser!.BanMs);
            if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            // ---------

            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToPage("/index");
            }
            else
                return Page();
        }
        /// <summary>
        /// Fill SiteUser property
        /// </summary>
        /// <returns>(true, true) if prop filled ok.</returns>
        private async Task<(bool, bool)> TryGetSiteUserAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return (false, false);

            SiteUser = await context.Users.AsNoTracking()
                    .Select(u => new User { Id = u.Id, BanMs = u.BanMs })
                    .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
