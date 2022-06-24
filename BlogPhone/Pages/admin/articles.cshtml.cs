using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BlogPhone.Models.Database;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class ArticlesModel : PageModel
    {
        readonly ApplicationContext context;
        public List<ArticleBlog>? Articles { get; set; }
        public User? SiteUser { get; set; }
        [BindProperty] public string? articleId { get; set; }
        public ArticlesModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin", "moder");
            if (!access) return BadRequest();

            if(id is not null) // for remove article
            {
                ArticleBlog? article = await context.ArticleBlogs.FirstOrDefaultAsync(a => a.Id.ToString() == id);
                if (article is null) return NotFound();

                context.ArticleBlogs.Remove(article);
                await context.SaveChangesAsync();

                return Redirect("/admin/articles");
            }

            List<ArticleBlog> arts = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            Articles = arts.TakeLast(3).ToList();
            Articles.Reverse();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (articleId is null) return NotFound();

            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");
            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            ArticleBlog? article = await context.ArticleBlogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id.ToString() == articleId);
            if (article is null) return RedirectToPage("/admin/articles");
            else Articles = new List<ArticleBlog>() { article };

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
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
