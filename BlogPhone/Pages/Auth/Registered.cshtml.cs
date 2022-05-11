using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPhone.Pages
{
    [Authorize]
    public class RegisteredModel : PageModel
    {
        public string? UserId { get; set; }
        public IActionResult OnGet()
        {
            UserId = HttpContext.User.FindFirst("Id")?.Value ?? "undefined";
            return Page();
        }


    }
}
