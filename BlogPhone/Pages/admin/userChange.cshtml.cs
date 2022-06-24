using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;
using BlogPhone.Models.Database;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class ChangeUserModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public User? ChangeUser { get; set; }
        [BindProperty] public int RoleAddDays { get; set; }
        public User? SiteUser { get; set; }
        public ChangeUserModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin"); // exist role check
            if (!access) return BadRequest();

            ChangeUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if(ChangeUser is null) return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnGetUnbanAsync(string id)
        {
            ChangeUser = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (ChangeUser is null) return NotFound();

            ChangeUser.BanMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            context.Users.Update(ChangeUser);
            await context.SaveChangesAsync();

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(ChangeUser is null) return BadRequest();

            User? oldUser = await context.Users.FirstOrDefaultAsync(u => u.Id == ChangeUser.Id);
            if(oldUser is null) return NotFound();

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
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            ViewData["email"] = SiteUser.Email;
            return (true, true);
        }
    }
}
