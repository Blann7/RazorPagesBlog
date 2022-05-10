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

        public IndexModel(ApplicationContext db)
        {
            context = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Articles = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            Articles.Reverse();
            return Page();
        }

        public string GetImageURLFromBytesArray(byte[]? imageData)
        {
            if (imageData is null) throw new Exception("DB FAILURE Index page");

            //FileContentResult image = new FileContentResult(imageData, "image/jpeg");
            //return image;

            // Конвертируем 
            string imreBase64Data = Convert.ToBase64String(imageData);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);

            return imgDataURL;
        }
    }
}