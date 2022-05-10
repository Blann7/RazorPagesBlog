using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages
{
    public class ArticleViewModel : PageModel
    {
        readonly ApplicationContext context;
        public ArticleBlog? Article { get; set; }

        public ArticleViewModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Article = await context.ArticleBlogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (Article is null) return BadRequest();
            
            return Page();
        }

        public string GetImageURLFromBytesArray(byte[]? imageData)
        {
            if (imageData is null) throw new Exception("DB FAILURE article view page");

            //FileContentResult image = new FileContentResult(imageData, "image/jpeg");
            //return image;

            // Конвертируем 
            string imreBase64Data = Convert.ToBase64String(imageData);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);

            return imgDataURL;
        }
    }
}
