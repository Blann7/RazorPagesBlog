using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages.admin
{
    public class userChangeModel : PageModel
    {
        ApplicationContext context;
        [BindProperty] public User? SiteUser { get; set; }
        public userChangeModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
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
            oldUser.BanDate = SiteUser.BanDate;

            context.Users.Update(oldUser);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/users");
        }
    }
}
