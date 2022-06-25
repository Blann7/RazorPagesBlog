using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using Microsoft.AspNetCore.Authorization;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class ReferralChangeModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        public User? SiteUser { get; set; }
        public User? ReferralUser { get; set; }
        [BindProperty] public Referral? Referral { get; set; }
        public ReferralChangeModel(ApplicationContext db, DbLogger dbLogger)
        {
            context = db;
            this.dbLogger = dbLogger;
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null) return NotFound();

            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();
            
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
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return NotFound();

            if (Referral is null) return BadRequest();

            Referral? oldReferral = await context.Referrals.FirstOrDefaultAsync(r => r.Id == Referral.Id);
            if (oldReferral is null) return NotFound();

            User? owner = await context.Users.AsNoTracking()
                .Select(u => new User() { Id = u.Id, Name = u.Name })
                .FirstOrDefaultAsync(u => u.Id == oldReferral.UserId);
            if(owner is null) return BadRequest();

            dbLogger.Add(SiteUser!.Id, SiteUser.Name!, LogViewer.Models.LogTypes.LogType.EDIT_REFERRAL,
                $"Изменил рефералку (id - {oldReferral.Id}) [Код {oldReferral.Code} -> {Referral.Code}], [Пригласил {oldReferral.InvitedUsers} -> {Referral.InvitedUsers}]");
            dbLogger.Add(owner.Id, owner.Name!, LogViewer.Models.LogTypes.LogType.EDIT_REFERRAL,
                $"Администратор {SiteUser.Name!} (Id - {SiteUser.Id}) изменил рефералку (id - {oldReferral.Id}) [Код {oldReferral.Code} -> {Referral.Code}], [Пригласил {oldReferral.InvitedUsers} -> {Referral.InvitedUsers}]");

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
