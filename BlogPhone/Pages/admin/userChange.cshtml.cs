using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages.admin
{
    public class ChangeUserModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public User? SiteUser { get; set; }
        public string? AdminEmail { get; set; }
        public ChangeUserModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idString)) return RedirectToPage("/auth/logout");

            User? adminUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Email = u.Email })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (adminUser?.Email is null) return BadRequest();
            AdminEmail = adminUser.Email;

            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if(SiteUser is null) return NotFound();
            return Page();
        }
        public async Task<IActionResult> OnGetUnbanAsync(string id)
        {
            SiteUser = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (SiteUser is null) return NotFound();

            SiteUser.BanDate = DateTime.UtcNow.ToString();

            context.Users.Update(SiteUser);
            await context.SaveChangesAsync();

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(SiteUser is null) return BadRequest();

            User? oldUser = await context.Users.FirstOrDefaultAsync(u => u.Id == SiteUser.Id);
            if(oldUser is null) return NotFound();

            oldUser.Name = SiteUser.Name;
            oldUser.Password = SiteUser.Password;
            oldUser.Email = SiteUser.Email;
            oldUser.Money = SiteUser.Money;
            oldUser.Role = SiteUser.Role;
            oldUser.RoleValidityDate = DateTime.UtcNow.AddDays(31).ToString();
            oldUser.BanDate = SiteUser.BanDate;

            context.Users.Update(oldUser);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/users");
        }
    }
}
