using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class BanUserModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        [BindProperty] public User? BanUser { get; set; }
        [BindProperty] public DateTime BanDate { get; set; }
        public User? SiteUser { get; set; }
        public BanUserModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            bool getInfoBanResult = await TryGetBanUserAsync(id);
            if (!getInfoBanResult) return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if (BanUser is null) return BadRequest();

            long banDt = DateTimeOffset.Parse(BanDate.ToString()).ToUnixTimeMilliseconds();

            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == BanUser.Id); // for ban user
            if(user is null) return NotFound();

            user.BanMs = banDt;
            user.Role = "user"; // reset role to user

            context.Users.Update(user);
            await context.SaveChangesAsync();

            dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.BAN,
               $"Заблокировал {user.Name} (Id - {user.Id}) до {BanDate.ToShortDateString()}");
            dbLogger.Add(user.Id, user.Name!, LogViewer.Models.LogTypes.LogType.BAN,
               $"Заблокирован администратором {SiteUser.Name} (Id - {SiteUser.Id}) до {BanDate.ToShortDateString()}");

            return RedirectToPage("/admin/users");
        }
        /// <summary>
        /// Fill BanUser property
        /// </summary>
        /// <returns>true if prop filled ok.</returns>
        private async Task<bool> TryGetBanUserAsync(string id)
        {
            BanUser = await context.Users.AsNoTracking()
                .Select(u => new Models.User { Id = u.Id, Name = u.Name, Email = u.Email, BanMs = u.BanMs })
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
                .Select(u => new User { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            if (!AccessChecker.RoleCheck(SiteUser!.Role, "admin")) return (false, true);

            ViewData["email"] = SiteUser.Email;
            return (true, true);
        }
    }
}
