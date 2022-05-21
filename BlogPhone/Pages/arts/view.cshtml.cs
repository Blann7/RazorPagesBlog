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
            // ban check
            string? userId = HttpContext.User.FindFirst("Id")?.Value;

            User? user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user is not null)
            {
                bool banned = AccessChecker.BanCheck(user.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            }
            // ---------


            Article = await context.ArticleBlogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (Article is null) return BadRequest();
            
            return Page();
        }
    }
}
