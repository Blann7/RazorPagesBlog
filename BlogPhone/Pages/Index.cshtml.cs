using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages
{
    public class IndexModel : PageModel
    {
        readonly ApplicationContext context;
        public List<ArticleBlog> Articles { get; set; } = new();
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }
        public IndexModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string? code)
        {
            TempLogFile.Create();
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                (bool, bool) getInfoResult = await TryGetSiteUserAsync();
                if (getInfoResult != (true, true)) return BadRequest();

                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser!.BanMs);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

            // Referral code -------------------------------------------------------------
            if (code is not null)
            {
                bool codeExist = await Referral.IsCodeAlreadyExist(code);
                if (codeExist) Response.Cookies.Append("bonus", code);
            }

            // "Показать больше" and "скрыть" buttons ------------------------------------
            if (Request.Cookies["indexLoad"] is null)
            {
                Response.Cookies.Append("indexLoad", "3");
                return RedirectToPage("/index");
            }
            // ---------------------------------------------------------------------------

            List<ArticleBlog> arts = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            if (Request.Cookies["indexLoad"] == "all") Articles = arts;
            else Articles = arts.TakeLast(int.Parse(Request.Cookies["indexLoad"]!)).ToList();
            Articles.Reverse();

            return Page();
        }
        public IActionResult OnPostMore() // "Показать больше" pressed
        {
            if (Request.Cookies["indexLoad"] == "3") Response.Cookies.Append("indexLoad", "10");
            else if (Request.Cookies["indexLoad"] == "10") Response.Cookies.Append("indexLoad", "20");
            else if (Request.Cookies["indexLoad"] == "20") Response.Cookies.Append("indexLoad", "30");
            else Response.Cookies.Append("indexLoad", "all");

            return RedirectToPage("/index");
        }
        public IActionResult OnPostLess() // "скрыть" pressed
        {
            Response.Cookies.Append("indexLoad", "3");

            return RedirectToPage("/index");
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
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanMs = u.BanMs })
                    .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}