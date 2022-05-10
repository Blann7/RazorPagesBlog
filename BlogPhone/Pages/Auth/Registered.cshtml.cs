using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models.Auth;

namespace BlogPhone.Pages
{
    public class RegisteredModel : PageModel
    {
        // https://localhost:7230/Auth/Registered/00000000-0000-0000-0000-000000000000
        public IActionResult OnGet(string regkey)
        {
            foreach (string key in RegisteredOptions.regKeys.ToList())
            {
                if (key == regkey)
                {
                    RegisteredOptions.regKeys.Remove(key);
                    return Page();
                }
            }
            return RedirectToPage("/Index");
        }
    }
}
