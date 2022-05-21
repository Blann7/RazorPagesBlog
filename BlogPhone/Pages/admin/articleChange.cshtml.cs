using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages.admin
{
    public class articlesChangeModel : PageModel
    {
        ApplicationContext context;
        public ArticleBlog? Article { get; set; }
        [BindProperty] public string? Label { get; set; }
        [BindProperty] public string? ArticleText { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }
        public articlesChangeModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
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
    }
}
