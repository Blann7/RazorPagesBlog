using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize(Roles = "admin")] // step 1 check role
    public class articlesModel : PageModel
    {
        ApplicationContext context;
        public List<ArticleBlog>? Articles { get; set; }
        public User? SiteUser { get; set; }
        public articlesModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");

            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            bool access = AccessChecker.RoleCheck(SiteUser?.Role, "admin"); // step 2 check role
            if (!access) return RedirectToPage("/auth/logout");

            Articles = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            Articles.Reverse();
            

            if(!string.IsNullOrEmpty(id)) // for remove article
            {
                ArticleBlog? article = await context.ArticleBlogs.FirstOrDefaultAsync(a => a.Id.ToString() == id);
                if (article is null) return NotFound();

                context.ArticleBlogs.Remove(article);
                await context.SaveChangesAsync();

                return RedirectToPage("/admin/articles");
            }

            return Page();
        }
    }
}
