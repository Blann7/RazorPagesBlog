using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class VRCModel : PageModel
    {
        readonly ApplicationContext context;
        public User? SiteUser { get; set; }
        public List<string> Results { get; set; } = new();
        public VRCModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser?.Role, "admin");
            if (!access) return BadRequest();

            // VRC --------------------------------------------------------------------
            await VRCAsync();

            if(Results.Count == 0)
                return Content("Проверено, никто не снят", "text/html", Encoding.UTF8);

            string res = "Проверено!";
            foreach (string s in Results)
            {
                res = res + "<br />" + s;
            }

            return Content(res, "text/html", Encoding.UTF8);
        }
        private async Task VRCAsync()
        {
            List<User> users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Role = u.Role, RoleValidityMs = u.RoleValidityMs })
                .Where(u => u.Role != "user").ToListAsync();

            List<User> expiratedUsers = new();

            foreach (User user in users)
            {
                if (user.RoleValidityMs is null)
                    throw new Exception("VRC: user.RoleValidityMs is null");

                long nowDt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                if (user.RoleValidityMs < nowDt)
                    expiratedUsers.Add(user);
            }

            if (expiratedUsers.Count > 0)
            {
                foreach (User user in expiratedUsers)
                {
                    User? fullUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                    if (fullUser is null)
                        throw new Exception("admin/VRC RoleValidityChecker: fullUser is null");

                    Results!.Add($"снят {fullUser.Name} (id: {fullUser.Id})");

                    fullUser.Role = "user";

                    context.Users.Update(fullUser);
                    await context.SaveChangesAsync();
                }
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
                .Select(u => new User { Id = u.Id, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
    }
}
