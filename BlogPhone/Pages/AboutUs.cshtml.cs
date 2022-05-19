using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages
{
    public class AboutUsModel : PageModel
    {
        ApplicationContext context;
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }

        public AboutUsModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.FindFirst("Id") is null) return RedirectToPage("/auth/logout");
                int Id = int.Parse(HttpContext.User.FindFirst("Id")!.Value);

                SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == Id);
                if (SiteUser is null) return RedirectToPage("/auth/logout");

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

            return Page();
        }
    }
}
