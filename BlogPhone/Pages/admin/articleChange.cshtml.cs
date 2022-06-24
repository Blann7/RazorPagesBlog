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
    public class ChangeArticleModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        public ArticleBlog? Article { get; set; }
        [BindProperty] public string? Label { get; set; }
        [BindProperty] public string? ArticleText { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }
        public User? SiteUser { get; set; }
        public ChangeArticleModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            Article = await context.ArticleBlogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id.ToString() == id);
            if (Article is null) return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            Article = await context.ArticleBlogs.FirstOrDefaultAsync(a => a.Id.ToString() == id);
            if (Article is null) return BadRequest();

            Article.Label = Label;
            Article.ArticleText = ArticleText;
            Article.SetShortArticleText();

            if(Image is not null)
                Article.ImageData = ImageWorking.GetImageDataFromIFormFile(Image);

            context.ArticleBlogs.Update(Article);
            await context.SaveChangesAsync();

            dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.CHANGE_ARTICLE, $"Изменил статью (id - {id}) - '{Label}'");
            return RedirectToPage("/admin/articles");
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

            ViewData["email"] = SiteUser.Email;
            return (true, true);
        }
    }
}
