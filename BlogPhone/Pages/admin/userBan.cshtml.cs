using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages.admin
{
    public class userBanModel : PageModel
    {
        ApplicationContext context;
        [BindProperty] public User? SiteUser { get; set; }
        public userBanModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            SiteUser = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (SiteUser is null) return NotFound();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(SiteUser is null) return NotFound();
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == SiteUser.Id);
            if(user is null) return NotFound();

            user.BanDate = SiteUser.BanDate;

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/users");
        }
    }
}
