using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages
{
    [Authorize(Roles = "admin, moder")] // step 1 check roles
    public class AddArticleModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public string? Label { get; set; }
        [BindProperty] public string? ArticleText { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }
        public byte[]? ImageData { get; set; }
        public string Message { get; set; } = "";

        public AddArticleModel(ApplicationContext db)
        {
            context = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/Auth/Logout");

            User? SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            bool access = AccessChecker.RoleCheck(SiteUser?.Role, "admin", "moder"); // step 2 check role
            if (!access) return RedirectToPage("/auth/logout");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(Image is not null)
                ImageData = ImageWorking.GetImageDataFromIFormFile(Image);
            else // default picture
                ImageData = await ImageWorking.GetImageDataFromDefaultImageAsync();

            // ---------------------------------------------

            if (Label != null && ArticleText != null && ImageData != null)
            {
                ArticleBlog ab = new (Label, ArticleText, ImageData);

                await context.ArticleBlogs.AddAsync(ab);
                await context.SaveChangesAsync();

                return RedirectToPage("/admin/articles");
            }
            else return BadRequest();
        }
    }
}
