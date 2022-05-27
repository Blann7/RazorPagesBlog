using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

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
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.FindFirst("Id") is null) return RedirectToPage("/auth/logout");
                int Id = int.Parse(HttpContext.User.FindFirst("Id")!.Value);

                SiteUser = await context.Users.AsNoTracking()
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanDate = u.BanDate })
                    .FirstOrDefaultAsync(u => u.Id == Id);
                if(SiteUser is null) return RedirectToPage("/auth/logout");

                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
                // ---------

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

            if (Request.Cookies["indexLoad"] is null)
            {
                Response.Cookies.Append("indexLoad", "3");
                return RedirectToPage("/index");
            }

            List<ArticleBlog> arts = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            if (Request.Cookies["indexLoad"] == "all") Articles = arts;
            else Articles = arts.TakeLast(int.Parse(Request.Cookies["indexLoad"]!)).ToList();

            return Page();
        }
        public IActionResult OnPostMore()
        {
            if (Request.Cookies["indexLoad"] == "3") Response.Cookies.Append("indexLoad", "10");
            else if (Request.Cookies["indexLoad"] == "10") Response.Cookies.Append("indexLoad", "20");
            else if (Request.Cookies["indexLoad"] == "20") Response.Cookies.Append("indexLoad", "30");
            else Response.Cookies.Append("indexLoad", "all");

            return RedirectToPage("/index");
        }
        public IActionResult OnPostLess()
        {
            Response.Cookies.Append("indexLoad", "3");

            return RedirectToPage("/index");
        }

    }
}