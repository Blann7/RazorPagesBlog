using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models.Auth;

namespace BlogPhone.Pages
{
    public class RegisteredModel : PageModel
    {

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
            return NotFound();
        }
    }
}
