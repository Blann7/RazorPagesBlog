using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages.Office
{
    [Authorize]
    public class UserModel : PageModel
    {
        readonly ApplicationContext context;
        public User? SiteUser { get; set; }

        public UserModel(ApplicationContext db)
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

            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            ViewData["email"] = SiteUser.Email;

            return (true, true);
        }
    }
}
