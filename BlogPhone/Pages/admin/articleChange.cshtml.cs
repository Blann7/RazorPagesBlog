using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BlogPhone.Models.Database;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class ChangeArticleModel : PageModel
    {
        readonly ApplicationContext context;
        public ArticleBlog? Article { get; set; }
        [BindProperty] public string? Label { get; set; }
        [BindProperty] public string? ArticleText { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }
        public User? SiteUser { get; set; }
        public ChangeArticleModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin", "moder");
            if (!access) return BadRequest();

            Article = await context.ArticleBlogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id.ToString() == id);
            if (Article is null) return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            Article = await context.ArticleBlogs.FirstOrDefaultAsync(a => a.Id.ToString() == id);
            if (Article is null) return NotFound();

            Article.Label = Label;
            Article.ArticleText = ArticleText;
            Article.SetShortArticleText();

            if(Image is not null)
                Article.ImageData = ImageWorking.GetImageDataFromIFormFile(Image);

            context.ArticleBlogs.Update(Article);
            await context.SaveChangesAsync();

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
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            ViewData["email"] = SiteUser.Email;
            return (true, true);
        }
    }
}
