using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using System.Text;

namespace BlogPhone.Pages
{
    public class RegisterModel : PageModel
    {
        ApplicationContext context;
        [BindProperty] public User SiteUser { get; set; } = new();

        public RegisterModel(ApplicationContext db)
        { 
            context = db; 
        }
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            string confirmPassword = Request.Form["passwordagain"];
            if (confirmPassword is null || SiteUser.Password != confirmPassword)
            {
                return Page();
            }

            // to avoid repetition
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if (user is not null) return Content("ѕользователь с таким именем уже есть!", "text/html", Encoding.UTF8);

            user = await context.Users.FirstOrDefaultAsync(u => u.Email == SiteUser.Email);
            if (user is not null) return Content("ѕользователь с такой почтой уже есть!", "text/html", Encoding.UTF8);
            //--------------------

            await context.Users.AddAsync(SiteUser);
            await context.SaveChangesAsync();

            return RedirectToPage("/Auth/Login");
        }
    }
}
