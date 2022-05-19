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
        public async Task OnGetAsync(string? id) // id is not null in case of delete selected account
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) RedirectToPage("/auth/logout");

            bool access = AccessChecker.Check(SiteUser?.Role, "admin"); // step 2 check role
            if (!access) RedirectToPage("/auth/logout");

            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);

            if (id is not null) await SetUserRole(id);

            HighRoleUsers = await context.Users.Where(u => u.Role != "user").ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if(user is null)
            {
                Message = $"Пользователь с id {UserId} не найден";
                return RedirectToPage("/admin/heads");
            }

            Message = $"Успешно! || {user.Name}: {user.Role} => {UserRole}";
            user.Role = UserRole;

            await context.SaveChangesAsync();

            return RedirectToPage("/admin/heads");
        }
        private async Task<IActionResult> SetUserRole(string id)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (user is null) return NotFound();

            user.Role = "user";
            await context.SaveChangesAsync();
            return RedirectToPage("/admin/heads");
        }
    }
}
