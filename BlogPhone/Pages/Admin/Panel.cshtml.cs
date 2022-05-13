using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages.Admin
{
    // AUTHORIZE WORKING!
    public class PanelModel : PageModel
    {
        ApplicationContext context;
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }

        public PanelModel(ApplicationContext db)
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
