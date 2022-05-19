using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages
{
    public class AddArtModel : PageModel
    {
        readonly ApplicationContext context;

        [BindProperty] public string? Label { get; set; }
        public string? ShortArticleText { get; set; }
        [BindProperty] public string? AricleText { get; set; }
        [BindProperty] public IFormFile? Image { get; set; }
        public byte[]? ImageData { get; set; }
        public string? PublishDate { get; set; }
        public string Message { get; set; } = "";

        public AddArtModel(ApplicationContext db)
        {
            context = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // ban check
            string? userId = HttpContext.User.FindFirst("Id")?.Value;

            User? user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user is not null)
            {
                bool banned = AccessChecker.BanCheck(user.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            }
            // ---------

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(Image is not null)
            {
                using (BinaryReader br = new BinaryReader(Image.OpenReadStream()))
                {
                    ImageData = br.ReadBytes((int)Image.Length);
                }
            }
            else
            {
                using (FileStream fs = new FileStream($"{Environment.CurrentDirectory}/wwwroot/images/default-thumbnail.jpg", FileMode.Open))
                {
                    byte[] fsbyte = new byte[fs.Length];
                    await fs.ReadAsync(fsbyte);

                    ImageData = fsbyte;
                }
            }

            if (!string.IsNullOrEmpty(AricleText) && AricleText.Length > 50)
            {
                for (int i = 0; i < 50; i++)
                {
                    ShortArticleText += AricleText[i];
                }
                ShortArticleText += "...";
            }
            else if (!string.IsNullOrEmpty(AricleText))
            {
                ShortArticleText = AricleText;
            }
            else BadRequest("article text undefined");

            PublishDate = DateTime.Now.ToShortDateString();

            // ---------------------------------------------

            if (Label != null && ShortArticleText != null && AricleText != null && ImageData != null && PublishDate != null)
            {
                ArticleBlog ab = new ArticleBlog(Label, ShortArticleText, AricleText, ImageData, PublishDate);
                await context.ArticleBlogs.AddAsync(ab);
                await context.SaveChangesAsync();
                Message = "Успешно добавлено";
                return Page();
            }
            else return BadRequest();
        }
    }
}
