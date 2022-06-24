using BlogPhone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models.Database;

namespace BlogPhone.Pages
{
    public class GoodsModel : PageModel
    {
        readonly ApplicationContext context;
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }
        public GoodsModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                (bool, bool) getInfoResult = await TryGetSiteUserAsync();
                if (getInfoResult != (true, true)) return BadRequest();

                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser!.BanMs);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
                // ---------

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

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
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanMs = u.BanMs, Role = u.Role })
                    .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
