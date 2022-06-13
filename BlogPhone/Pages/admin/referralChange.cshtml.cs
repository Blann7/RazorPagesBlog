using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class ReferralChangeModel : PageModel
    {
        private readonly ApplicationContext context;
        public User? SiteUser { get; set; }
        public User? ReferralUser { get; set; }
        [BindProperty] public Referral? Referral { get; set; }
        public ReferralChangeModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null) return NotFound();

            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin");
            if (!access) return BadRequest();

            if (SiteUser!.FullDostup == false) return NotFound();

            Referral = await context.Referrals.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if(Referral is null) return NotFound();

            ReferralUser = await context.Users.AsNoTracking()
                .Select(u => new User() { Id = u.Id, Name = u.Name, Email = u.Email })
                .FirstOrDefaultAsync(u => u.Id == Referral!.UserId);
            if(ReferralUser is null) return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (Referral is null) return BadRequest();

            Referral? oldReferral = await context.Referrals.FirstOrDefaultAsync(r => r.Id == Referral.Id);
            if (oldReferral is null) return NotFound();

            oldReferral.Code = Referral.Code;
            oldReferral.InvitedUsers = Referral.InvitedUsers;

            context.Referrals.Update(oldReferral);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/referrals");
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

            ViewData["email"] = SiteUser.Email;

            return (true, true);
        }
    }
}
