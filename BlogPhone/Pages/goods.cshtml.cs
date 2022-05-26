using BlogPhone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPhone.Pages
{
    public class GoodsModel : PageModel
    {
        readonly ApplicationContext context;
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }
        public GoodsModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.FindFirst("Id") is null) return RedirectToPage("/auth/logout");
                int Id = int.Parse(HttpContext.User.FindFirst("Id")!.Value);

                SiteUser = await context.Users.AsNoTracking()
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanDate = u.BanDate, Role = u.Role })
                    .FirstOrDefaultAsync(u => u.Id == Id);
                if (SiteUser is null) return RedirectToPage("/auth/logout");

                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
                // ---------

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
            }

            return Page();
        }
    }
}