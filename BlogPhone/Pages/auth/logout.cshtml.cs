using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages.Auth
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        public User? SiteUser { get; set; }
        public LogoutModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
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
                dbLogger.Add(SiteUser.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.LOGOUT, "Вышел");
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
                    .Select(u => new User { Id = u.Id, Name = u.Name, BanMs = u.BanMs })
                    .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
