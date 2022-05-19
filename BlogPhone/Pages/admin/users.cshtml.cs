using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize(Roles = "admin")] // step 1 check role
    public class usersModel : PageModel
    {
        ApplicationContext context;
        public List<User>? Users { get; set; }
        public User? SiteUser { get; set; }
        public usersModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");

            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            bool access = AccessChecker.Check(SiteUser?.Role, "admin"); // step 2 check role
            if (!access) return RedirectToPage("/auth/logout");

            Users = await context.Users.AsNoTracking().ToListAsync();

            return Page();
        }
    }
}
