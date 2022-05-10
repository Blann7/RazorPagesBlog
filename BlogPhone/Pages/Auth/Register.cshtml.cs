using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            string confirmPassword = Request.Form["passwordagain"];
            if (confirmPassword is null || SiteUser.Password != confirmPassword)
            {
                return Page();
            }

            // to avoid repetition
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if (user is not null) return Content("Пользователь с таким именем уже есть!", "text/html", Encoding.UTF8);

            user = await context.Users.FirstOrDefaultAsync(u => u.Email == SiteUser.Email);
            if (user is not null) return Content("Пользователь с такой почтой уже есть!", "text/html", Encoding.UTF8);
            //--------------------

            await context.Users.AddAsync(SiteUser);
            await context.SaveChangesAsync();

            //await SetClaimsAsync();

            return Content("Вы успешно зарегистрировались!", "text/html", Encoding.UTF8);
        }

        public async Task SetClaimsAsync()
        {
            if (SiteUser.Name is null || SiteUser.Email is null) return;

            List<Claim> claims = new List<Claim> { 
                new Claim(ClaimTypes.Name, SiteUser.Name), 
                new Claim(ClaimTypes.Email, SiteUser.Email)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));
        }
    }
}
