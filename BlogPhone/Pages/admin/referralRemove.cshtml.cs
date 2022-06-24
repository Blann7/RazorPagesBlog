using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages.admin
{
    public class ReferralRemoveModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        public User? SiteUser { get; set; }
        public User? ReferralUser { get; set; }
        [BindProperty] public Referral? Referral { get; set; }
        public ReferralRemoveModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if (id is null) return NotFound();

            Referral = await context.Referrals.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (Referral is null) return BadRequest();

            ReferralUser = await context.Users.AsNoTracking()
                .Select(u => new User() { Id = u.Id, Name = u.Name, Email = u.Email })
                .FirstOrDefaultAsync(u => u.Id == Referral!.UserId);
            if (ReferralUser is null) return BadRequest();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if (Referral is null) return BadRequest();

            Referral? referral = await context.Referrals.FirstOrDefaultAsync(r => r.Id == Referral.Id);
            if (referral is null) return NotFound();

            context.Referrals.Remove(referral);
            await context.SaveChangesAsync();

            dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.DELETE_REFERRAL,
               $"Удалил рефералку (id - {referral.Id}) [Код {referral.Code}], [Пригласил {Referral.InvitedUsers}");

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
                .Select(u => new User { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role, FullDostup = u.FullDostup })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            if (!AccessChecker.RoleCheck(SiteUser!.Role, "admin")) return (false, true);
            if (SiteUser!.FullDostup == false) return (false, true);

            ViewData["email"] = SiteUser.Email;

            return (true, true);
        }
    }
}
