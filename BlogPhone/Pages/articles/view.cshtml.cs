using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using BlogPhone.Models.Database;

namespace BlogPhone.Pages
{
    public class ViewModel : PageModel
    {
        private readonly ApplicationContext context;
        public ArticleBlog? Article { get; set; }
        public User? SiteUser { get; set; }
        public ViewModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult == (true, true))
            {
                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser!.BanMs);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            }

            Article = await context.ArticleBlogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (Article is null) return NotFound();
            
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
