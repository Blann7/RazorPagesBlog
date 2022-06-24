using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class ArticlesModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        public List<ArticleBlog>? Articles { get; set; }
        public User? SiteUser { get; set; }
        [BindProperty] public string? articleId { get; set; }
        public ArticlesModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if(id is not null) // for remove article
            {
                ArticleBlog? article = await context.ArticleBlogs.FirstOrDefaultAsync(a => a.Id.ToString() == id);
                if (article is null) return BadRequest();

                context.ArticleBlogs.Remove(article);
                await context.SaveChangesAsync();

                dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.DELETE_ARTICLE, 
                    $"Удалил статью (id - {id}) - '{article.Label}'");
                return Redirect("/admin/articles");
            }

            List<ArticleBlog> arts = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            Articles = arts.TakeLast(3).ToList();
            Articles.Reverse();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if (articleId is null) return BadRequest();

            /// test full user

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
                .Select(u => new User { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            if (!AccessChecker.RoleCheck(SiteUser!.Role, "admin", "moder")) return (false, true);

            return (true, true);
        }
        //private async Task<(bool, bool)> TryGetFULLSiteUserAsync()
        //{
        //    string? idString = HttpContext.User.FindFirst("Id")?.Value;
        //    if (idString is null) return (false, false);

        //    SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);
        //    if (SiteUser is null) return (true, false);

        //    if (!AccessChecker.RoleCheck(SiteUser!.Role, "admin", "moder")) return (false, true);

        //    return (true, true);
        //}
    }
}
