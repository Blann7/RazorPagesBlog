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
        int roleCost;
        public string? Role { get; set; }
        public User? SiteUser { get; set; }
        public BuyRoleModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(string role)
        {
            if (role != "admin" && role != "moder" && role != "standart" && role != "user") return NotFound();
            Role = role;

            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            // ban check
            bool banned = AccessChecker.BanCheck(SiteUser!.BanMs);
            if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
            // ---------

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string role)
        {
            if (role != "admin" && role != "moder" && role != "standart" && role != "user") return NotFound();
            Role = role;

            (bool, bool) getInfoResult = await TryGetFULLSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            SetRoleCost();

            if (SiteUser!.Money < roleCost)
                return Content("You have not enough money", "text/html", Encoding.UTF8);
            else
            {
                SiteUser.Money -= roleCost;
                SiteUser.Role = Role;
                if (Role != "user")
                    SiteUser.RoleValidityMs = DateTimeOffset.UtcNow.AddDays(31).ToUnixTimeMilliseconds();
                else
                    SiteUser.RoleValidityMs = DateTimeOffset.UtcNow.AddYears(100).ToUnixTimeMilliseconds();

                context.Users.Update(SiteUser);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("/goods");
        }
        /// <summary>
        /// Fill SiteUser property
        /// </summary>
        /// <returns>(true, true) if prop filled ok.</returns>
        private async Task<(bool, bool)> TryGetSiteUserAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return (false, false);

            SiteUser = await context.Users.AsNoTracking()
                .Select(u => new Models.User { Id = u.Id, BanMs = u.BanMs }) // selected info
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
        /// <summary>
        /// Fill SiteUser with all info property
        /// </summary>
        /// <returns>(true, true) if prop filled ok.</returns>
        private async Task<(bool, bool)> TryGetFULLSiteUserAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return (false, false);

            SiteUser = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == idString); // full info
            if (SiteUser is null) return (true, false);

            return (true, true);
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
