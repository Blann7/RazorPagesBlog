using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize(Roles = "admin")] // step 1 check role
    public class UsersModel : PageModel
    {
        readonly ApplicationContext context;
        public List<User>? Users { get; set; }
        public User? SiteUser { get; set; }
        [BindProperty] public string? UserId { get; set; }
        public UsersModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");

            SiteUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            bool access = AccessChecker.RoleCheck(SiteUser?.Role, "admin"); // step 2 check role
            if (!access) return RedirectToPage("/auth/logout");

            Users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Name = u.Name, BanDate = u.BanDate })
                .Where(u => u.Id < 16).ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");

            SiteUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            Users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Name = u.Name, BanDate = u.BanDate })
                .Where(u => u.Id.ToString() == UserId).ToListAsync();

            return Page();
        }
    }
}
