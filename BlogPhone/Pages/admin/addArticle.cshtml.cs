using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages
{
    [Authorize]
    public class AddArticleModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public string? Label { get; set; }
        [BindProperty] public string? ArticleText { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }
        public User? SiteUser { get; set; }
        public byte[]? ImageData { get; set; }
        public string Message { get; set; } = "";

        public AddArticleModel(ApplicationContext db)
        {
            context = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin", "moder");
            if (!access) return RedirectToPage("/auth/logout");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(Image is not null)
                ImageData = ImageWorking.GetImageDataFromIFormFile(Image);

            // ---------------------------------------------

            if (Label != null && ArticleText != null)
            {
                ArticleBlog ab = new (Label, ArticleText, ImageData);

                await context.ArticleBlogs.AddAsync(ab);
                await context.SaveChangesAsync();

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
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            ViewData["email"] = SiteUser.Email;

            return (true, true);
        }
    }
}
