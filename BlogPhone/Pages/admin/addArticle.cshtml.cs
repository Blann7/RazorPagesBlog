using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages
{
    [Authorize]
    public class AddArticleModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        [BindProperty] public string? Label { get; set; }
        [BindProperty] public string? ArticleText { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }
        public User? SiteUser { get; set; }
        public byte[]? ImageData { get; set; }
        public string Message { get; set; } = "";

        public AddArticleModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if (Image is not null)
                ImageData = ImageWorking.GetImageDataFromIFormFile(Image);

            if (Label != null && ArticleText != null)
            {
                ArticleBlog ab = new (Label, ArticleText, ImageData);

                await context.ArticleBlogs.AddAsync(ab);
                await context.SaveChangesAsync();

                dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.ADD_ARTICLE, $"Добавил статью '{Label}'");
                return RedirectToPage("/admin/articles");
            }
            else return BadRequest();
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
