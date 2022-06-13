using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class ReferralsModel : PageModel
    {
        private readonly ApplicationContext context;
        public User? SiteUser { get; set; }
        public List<Referral>? Referrals { get; set; }
        public List<User> Users { get; set; } = new ();
        [BindProperty] public string? Code { get; set; }
        public ReferralsModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin");
            if (!access) return BadRequest();

            if (SiteUser!.FullDostup == false) return NotFound();

            await GetReferralsAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            Referral? referral = await context.Referrals.AsNoTracking().FirstOrDefaultAsync(r => r.Code == Code);
            if (referral is null) return NotFound();

            User? user = await context.Users.AsNoTracking()
                .Select(u => new User() { Id = u.Id, Name = u.Name })
                .FirstOrDefaultAsync(u => u.Id == referral.UserId);
            if (user is null) return NotFound();

            Referrals = new List<Referral>() { referral };
            Users.Add(user);

            return Page();
        }
        /// <summary>
        /// Fill Referrals property
        /// </summary>
        /// <returns>Returns TRUE if success, false - failure</returns>
        private async Task GetReferralsAsync()
        {
            Referrals = await context.Referrals
                .AsNoTracking()
                .OrderByDescending(r => r.InvitedUsers)
                .ToListAsync();

            foreach (Referral referral in Referrals) // to show additional information
            {
                User? user = await context.Users
                    .AsNoTracking()
                    .Select(u => new User() { Id = u.Id, Name = u.Name })
                    .FirstOrDefaultAsync(u => u.Id == referral.UserId);
                if (user is null) throw new Exception("GetReferralsAsync: user is null");

                Users.Add(user);
            }
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
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role, FullDostup = u.FullDostup })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
