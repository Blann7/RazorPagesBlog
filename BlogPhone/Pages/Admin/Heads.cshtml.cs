using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages.Admin
{
    [Authorize(Roles = "admin")] // step 1 check role
    public class HeadsModel : PageModel
    {
        ApplicationContext context;
        public string? Message { get; set; } = "Изменить роль по ID";
        [BindProperty] public int? UserId { get; set; }
        [BindProperty] public string? UserRole { get; set; }
        public List<User> HighRoleUsers { get; private set; } = new();
        public User? SiteUser { get; set; }
        public HeadsModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string? id) // id is not null in case of delete selected account
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");

            SiteUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Role = u.Role, Email = u.Email })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            bool access = AccessChecker.RoleCheck(SiteUser?.Role, "admin"); // step 2 check role
            if (!access) RedirectToPage("/auth/logout");

            if (id is not null) await SetUserRole(id); // if "снять с поста" button pressed

            HighRoleUsers = await context.Users
                .Select(u => new User { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role, RoleValidityDate = u.RoleValidityDate })
                .Where(u => u.Role != "user").ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if(user is null)
            {
                Message = $"Пользователь с id {UserId} не найден";
                return RedirectToPage("/admin/heads");
            }

            Message = $"Вечная роль выдана! || {user.Name}: {user.Role} => {UserRole}";
            user.Role = UserRole;
            user.RoleValidityDate = DateTime.UtcNow.AddYears(100).ToString();

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/heads");
        }
        private async Task<IActionResult> SetUserRole(string id)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (user is null) return NotFound();

            user.Role = "user";

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/heads");
        }
    }
}
