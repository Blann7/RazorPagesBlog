using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPhone.Pages.Auth
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        public bool Success { get; set; } = false;
        public async Task<IActionResult> OnGetAsync()
        {
            if(HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Success = true;
                return RedirectToPage("/index");
            }
            else
            {
                return Page();
            }
        }
    }
}
