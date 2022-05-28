using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize]
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
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser?.Role, "admin");
            if (!access) return BadRequest();

            Users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Name = u.Name, BanDate = u.BanDate })
                .Where(u => u.Id < 16).ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            Users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Name = u.Name, BanDate = u.BanDate })
                .Where(u => u.Id.ToString() == UserId).ToListAsync();

            return Page();
        }
        /// <summary>
        /// Fill SiteUser property
        /// </summary>
        /// <returns>(true, true) if prop filled ok.</returns>
        private async Task<(bool, bool)> TryGetSiteUserAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return (false, false);

            SiteUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
