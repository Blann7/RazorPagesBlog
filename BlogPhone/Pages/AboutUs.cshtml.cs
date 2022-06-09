using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages
{
    public class AboutUsModel : PageModel
    {
        readonly ApplicationContext context;
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }
        public AboutUsInfo? AUI { get; set; }
        public AboutUsModel(ApplicationContext db)
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
                bool banned = AccessChecker.BanCheck(SiteUser!.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
                // ---------

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

            AUI = await context.AboutUsPage.AsNoTracking().FirstOrDefaultAsync();

            //await ResetAboutUsInfoDataBase();

            return Page();
        }
        /// <summary>
        /// Remove all content in AboutUsInfo table, than add one work line
        /// </summary>
        private async Task ResetAboutUsInfoDataBase()
        {
            List<AboutUsInfo> aui = await context.AboutUsPage.ToListAsync();
            foreach (AboutUsInfo a in aui) // Delete all content
            {
                context.AboutUsPage.Remove(a);
            }

            AboutUsInfo AUI = new() // add work line
            {
                P1_ImageData = null,
                P1_Title = "title 1",
                P1_Text = "text 1",
                P2_ImageData = null,
                P2_Title = "title 2",
                P2_Text = "text 2"
            };
            await context.AboutUsPage.AddAsync(AUI);
            await context.SaveChangesAsync();
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
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanDate = u.BanDate, Role = u.Role })
                    .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
