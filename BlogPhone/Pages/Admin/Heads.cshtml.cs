using BlogPhone.Models;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages.Admin
{
    [Authorize] 
    public class HeadsModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        public string? Message { get; set; } = "Изменить роль по ID";
        [BindProperty] public int? UserId { get; set; }
        [BindProperty] public string? UserRole { get; set; }
        [BindProperty] public bool UserFullDostup { get; set; } = false;
        public List<User> HighRoleUsers { get; private set; } = new();
        public User? SiteUser { get; set; }
        public HeadsModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }
        public async Task<IActionResult> OnGetAsync(string? id) // id is not null in case of delete selected account
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if (id is not null && SiteUser!.FullDostup) await SetUserRole(id); // if "снять с поста" button pressed

            HighRoleUsers = await context.Users
                .Select(u => new User { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role, RoleValidityMs = u.RoleValidityMs, FullDostup = u.FullDostup })
                .Where(u => u.Role != "user").ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();
            if (SiteUser!.FullDostup == false) return NotFound();

            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if(user is null)
            {
                Message = $"Пользователь с id {UserId} не найден";
                return RedirectToPage("/admin/heads");
            }

            Message = $"Вечная роль выдана! || {user.Name}: {user.Role} => {UserRole}";
            user.Role = UserRole;
            user.RoleValidityMs = DateTimeOffset.UtcNow.AddYears(100).ToUnixTimeMilliseconds();

            if (UserFullDostup) user.FullDostup = true;
            else user.FullDostup = false;

            context.Users.Update(user);
            await context.SaveChangesAsync();

            dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.EDIT_HEADS, 
                $"Выдал вечную роль {UserRole} пользователю {user.Name} (id - {user.Id})");
            return RedirectToPage("/admin/heads");
        }
        private async Task<IActionResult> SetUserRole(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();
            if (SiteUser!.FullDostup == false) return NotFound();

            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (user is null) return NotFound();

            user.Role = "user";

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/heads");
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
                .Select(u => new User { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role, FullDostup = u.FullDostup })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            if (!AccessChecker.RoleCheck(SiteUser!.Role, "admin")) return (false, true);

            return (true, true);
        }
    }
}
