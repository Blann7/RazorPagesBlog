using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages.Office
{
    [Authorize]
    public class UserModel : PageModel
    {
        ApplicationContext context;
        public User? SiteUser { get; set; }

        public UserModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.FindFirst("Id") is null) return RedirectToPage("/Auth/Login");
            int userId = int.Parse(HttpContext.User.FindFirst("Id")!.Value);

            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if(SiteUser is null) return RedirectToPage("/Auth/Login");

            // ban check
            bool banned = AccessChecker.BanCheck(SiteUser.BanDate);
            if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            // ---------

            return Page();
        }
    }
}
