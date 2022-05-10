using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPhone.Pages
{
    [Authorize]
    public class RegisteredModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }


    }
}
