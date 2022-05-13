using BlogPhone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPhone.Pages
{
    public class RatesModel : PageModel
    {
        ApplicationContext context;
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }

        public RatesModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.FindFirst("Id") is null) return RedirectToPage("/Auth/Logout");
                int Id = int.Parse(HttpContext.User.FindFirst("Id")!.Value);

                SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == Id);
                if (SiteUser is null) return RedirectToPage("/Auth/Logout");

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

            return Page();
        }
    }
}
