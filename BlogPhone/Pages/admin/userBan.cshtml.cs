using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class BanUserModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public User? BanUser { get; set; }
        public User? SiteUser { get; set; }
        public BanUserModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin"); // exist role check
            if (!access) return BadRequest();

            bool getInfoBanResult = await TryGetBanUserAsync(id);
            if (!getInfoBanResult) return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(BanUser is null) return NotFound();

            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == BanUser.Id); // for ban user
            if(user is null) return NotFound();

            user.BanDate = BanUser.BanDate;
            user.Role = "user"; // reset role to user

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/users");
        }
        /// <summary>
        /// Fill BanUser property
        /// </summary>
        /// <returns>true if prop filled ok.</returns>
        private async Task<bool> TryGetBanUserAsync(string id)
        {
            BanUser = await context.Users.AsNoTracking()
                .Select(u => new Models.User { Id = u.Id, Name = u.Name, Email = u.Email, BanDate = u.BanDate })
                .FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (BanUser is null) return false;

            return true;
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

            ViewData["email"] = SiteUser.Email;
            return (true, true);
        }
    }
}
