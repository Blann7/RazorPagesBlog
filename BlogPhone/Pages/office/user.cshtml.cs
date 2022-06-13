using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Pages.Office
{
    [Authorize]
    public class UserModel : PageModel
    {
        readonly ApplicationContext context;
        public User? SiteUser { get; set; }
        public Referral? ReferralUser { get; set; }
        public UserModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            (bool, bool) getInfoResult = await TryGetFULLSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            // ban check
            bool banned = AccessChecker.BanCheck(SiteUser!.BanMs);
            if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);

            await TryGetReferralUserAsync(); // Referral

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            Referral referral = new Referral(SiteUser!.Id);

            await context.Referrals.AddAsync(referral);
            await context.SaveChangesAsync();

            return RedirectToPage("/office/user");
        }
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            Referral? referral = await context.Referrals.FirstOrDefaultAsync(r => r.UserId == SiteUser!.Id); // full loading (w/o .Select)
            if (referral is null) return Content("refferal is null", "text/html");

            context.Referrals.Remove(referral);
            await context.SaveChangesAsync();

            return RedirectToPage("/office/user");
        }
        /// <summary>
        /// Fill ReferralUser property
        /// </summary>
        /// <returns>true if prop filled ok.</returns>
        private async Task TryGetReferralUserAsync()
        {
            ReferralUser = await context.Referrals
                .AsNoTracking()
                .Select(r => new Referral() { UserId = r.UserId, Code = r.Code, InvitedUsers = r.InvitedUsers })
                .FirstOrDefaultAsync(r => r.UserId == SiteUser!.Id);
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
                .Select(u => new User() { Id = u.Id })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            ViewData["email"] = SiteUser.Email;

            return (true, true);
        }
        /// <summary>
        /// Fill SiteUser property
        /// </summary>
        /// <returns>(true, true) if prop filled ok.</returns>
        private async Task<(bool, bool)> TryGetFULLSiteUserAsync()
        {
            string? idString = HttpContext.User.FindFirst("Id")?.Value;
            if (idString is null) return (false, false);

            SiteUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            ViewData["email"] = SiteUser.Email;

            return (true, true);
        }
    }
}
