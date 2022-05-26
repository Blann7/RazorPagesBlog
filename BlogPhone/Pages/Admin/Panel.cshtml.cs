using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.Admin
{
    [Authorize(Roles = "admin, moder")] // step 1 check role
    public class PanelModel : PageModel
    {
        readonly ApplicationContext context;
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }

        public PanelModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/Auth/Logout");

            SiteUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Role = u.Role, Email = u.Email })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if(SiteUser is null) return RedirectToPage("/Auth/Logout");

            bool access = AccessChecker.RoleCheck(SiteUser.Role, "admin", "moder"); // step 2 check role
            if (!access) return RedirectToPage("/auth/logout");

            /* 
             * if step 1 check is ok, but step 2 check is not ok -
             * we redirecting user to logout page.
             * It is to avoid using of panel by demoted admins/moders
             */

            return Page();
        }
    }
}
