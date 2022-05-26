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
            Articles = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            Articles.Reverse();

            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.FindFirst("Id") is null) return RedirectToPage("/auth/logout");
                int Id = int.Parse(HttpContext.User.FindFirst("Id")!.Value);

                SiteUser = await context.Users.AsNoTracking()
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanDate = u.BanDate })
                    .FirstOrDefaultAsync(u => u.Id == Id);
                if(SiteUser is null) return RedirectToPage("/auth/logout");


                //----------
                //if (SiteUser.Id == 108)
                //{
                //    User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == 108);
                //    if (user is null) return Page();
                //    user.RoleValidityDate = DateTime.UtcNow.AddMinutes(-5).ToString();

                //    await context.SaveChangesAsync();
                //}
                //--------

                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
                // ---------

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

            return Page();
        }
    }
}