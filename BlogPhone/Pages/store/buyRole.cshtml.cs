using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BlogPhone.Pages.store
{
    [Authorize]
    public class BuyRoleModel : PageModel
    {
        readonly ApplicationContext context;
        public string? Role { get; set; }
        int roleCost;
        public BuyRoleModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string role)
        {
            if (role != "admin" && role != "moder" && role != "standart" && role != "user") return NotFound();
            Role = role;

            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");

            User? user = await context.Users.AsNoTracking()
                .Select(u => new Models.User { Id = u.Id, BanDate = u.BanDate })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if(user is null) return RedirectToPage("/auth/logout");

            // ban check
            bool banned = AccessChecker.BanCheck(user.BanDate);
            if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            // ---------

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string role)
        {
            if (role != "admin" && role != "moder" && role != "standart" && role != "user") return NotFound();
            Role = role;

            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return RedirectToPage("/auth/logout");

            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (user is null) return NotFound();

            SetRoleCost();

            if (user.Money < roleCost)
                return Content("You have not enough money", "text/html", Encoding.UTF8);
            else
            {
                user.Money -= roleCost;
                user.Role = Role;
                if(Role != "user")
                    user.RoleValidityDate = DateTime.UtcNow.AddDays(31).ToString();
                else
                    user.RoleValidityDate = DateTime.UtcNow.AddYears(100).ToString();

                context.Users.Update(user);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("/goods");
        }

        private void SetRoleCost()
        {
            if (Role == "admin") roleCost = AccessChecker.ADMIN_COST;
            if (Role == "moder") roleCost = AccessChecker.MODER_COST;
            if (Role == "standart") roleCost = AccessChecker.STANDART_COST;
            if (Role == "user") roleCost = AccessChecker.USER_COST;

            if(roleCost == 0 && Role != "user")
            {
                throw new Exception("role is null store/buyRole.cshtml.cs. Method - void SetRoleCost()");
            }
        }
    }
}
