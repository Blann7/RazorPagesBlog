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
    public class ChangeUserModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        [BindProperty] public User? ChangeUser { get; set; }
        [BindProperty] public int RoleAddDays { get; set; }
        public User? SiteUser { get; set; }
        public ChangeUserModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            ChangeUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if(ChangeUser is null) return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnGetUnbanAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            ChangeUser = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (ChangeUser is null) return NotFound();

            ChangeUser.BanMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            context.Users.Update(ChangeUser);
            await context.SaveChangesAsync();

            dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.UNBAN,
               $"Разблокировал {ChangeUser.Name} (Id - {ChangeUser.Id})");
            dbLogger.Add(ChangeUser.Id, ChangeUser.Name!, LogViewer.Models.LogTypes.LogType.UNBAN,
               $"Разблокирован администратором {SiteUser.Name!} (Id - {SiteUser.Id})");

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            if (ChangeUser is null) return BadRequest();

            User? oldUser = await context.Users.FirstOrDefaultAsync(u => u.Id == ChangeUser.Id);
            if(oldUser is null) return NotFound();

            dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.EDIT_USER,
               $"Изменил статистику: {ChangeUser.Name} (Id - {ChangeUser.Id}) [name {oldUser.Name} -> {ChangeUser.Name}] " +
               $"[pass {oldUser.Password} -> {ChangeUser.Password}] [email {oldUser.Email} -> {ChangeUser.Email}] " +
               $"[money {oldUser.Money} -> {ChangeUser.Money}] [role {oldUser.Role} -> {ChangeUser.Role}] [add days {RoleAddDays}]");

            dbLogger.Add(oldUser.Id, oldUser.Name!, LogViewer.Models.LogTypes.LogType.EDIT_USER,
               $"Администратор {SiteUser.Name!} (Id - {SiteUser!.Id}) изменил статистику: {ChangeUser.Name} (Id - {ChangeUser.Id}) " +
               $"[name {oldUser.Name} -> {ChangeUser.Name}] " +
               $"[pass {oldUser.Password} -> {ChangeUser.Password}] [email {oldUser.Email} -> {ChangeUser.Email}] " +
               $"[money {oldUser.Money} -> {ChangeUser.Money}] [role {oldUser.Role} -> {ChangeUser.Role}] [add days {RoleAddDays}]");

            oldUser.Name = ChangeUser.Name;
            oldUser.Password = ChangeUser.Password;
            oldUser.Email = ChangeUser.Email;
            oldUser.Money = ChangeUser.Money;

            if(oldUser.Role != ChangeUser.Role) // if role changed
            {
                oldUser.Role = ChangeUser.Role;
                oldUser.RoleValidityMs = DateTimeOffset.UtcNow.AddDays(31).ToUnixTimeMilliseconds();
            }
            else if(RoleAddDays != 0 && oldUser.RoleValidityMs is not null) // if days added
            {
                long dt = DateTimeOffset
                    .FromUnixTimeMilliseconds((long)oldUser.RoleValidityMs)
                    .AddDays(RoleAddDays)
                    .ToUnixTimeMilliseconds();

                oldUser.RoleValidityMs = dt;
            }

            context.Users.Update(oldUser);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/users");
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
