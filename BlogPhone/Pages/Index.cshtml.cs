using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages
{
    public class IndexModel : PageModel
    {
        readonly ApplicationContext context;
        private static bool VRC_IsWorking = false;
        public List<ArticleBlog> Articles { get; set; } = new();
        public bool IsAuthorize { get; set; } = false;
        public User? SiteUser { get; set; }
        public IndexModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
            {
                (bool, bool) getInfoResult = await TryGetSiteUserAsync();
                if (getInfoResult != (true, true)) return BadRequest();

                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser!.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
                // ---------

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;

                await VRCAsync(); // Validate Roles Checker
                VRC_IsWorking = false;
            }

            // "Показать больше" and "скрыть" buttons ------------------------------------
            if (Request.Cookies["indexLoad"] is null)
            {
                Response.Cookies.Append("indexLoad", "3");
                return RedirectToPage("/index");
            }
            // ---------------------------------------------------------------------------

            List<ArticleBlog> arts = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            if (Request.Cookies["indexLoad"] == "all") Articles = arts;
            else Articles = arts.TakeLast(int.Parse(Request.Cookies["indexLoad"]!)).ToList();
            Articles.Reverse();

            //await ResetAboutUsInfoDataBase();

            return Page();
        }
        public IActionResult OnPostMore() // "Показать больше" pressed
        {
            if (Request.Cookies["indexLoad"] == "3") Response.Cookies.Append("indexLoad", "10");
            else if (Request.Cookies["indexLoad"] == "10") Response.Cookies.Append("indexLoad", "20");
            else if (Request.Cookies["indexLoad"] == "20") Response.Cookies.Append("indexLoad", "30");
            else Response.Cookies.Append("indexLoad", "all");

            return RedirectToPage("/index");
        }
        public IActionResult OnPostLess() // "скрыть" pressed
        {
            Response.Cookies.Append("indexLoad", "3");

            return RedirectToPage("/index");
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
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanDate = u.BanDate })
                    .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            return (true, true);
        }
        /// <summary>
        /// Remove all content in AboutUsInfo table, than add work line
        /// </summary>
        private async Task ResetAboutUsInfoDataBase()
        {
            List<AboutUsInfo> aui = await context.AboutUsPage.ToListAsync();
            foreach (AboutUsInfo a in aui) // Delete all content
            {
                context.AboutUsPage.Remove(a);
            }

            AboutUsInfo AUI = new() // add work line
            {
                P1_ImageData = null,
                P1_Title = "title 1",
                P1_Text = "text 1",
                P2_ImageData = null,
                P2_Title = "title 2",
                P2_Text = "text 2"
            };
            await context.AboutUsPage.AddAsync(AUI);
            await context.SaveChangesAsync();
        }
        // ValidateRoleChecker -----------------------------------------------------------------------------------------------------
        public async Task VRCAsync()
        {
            if (VRC_IsWorking) return; // to avoid parallel working
            else VRC_IsWorking = true;

            List<User> users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Role = u.Role, RoleValidityDate = u.RoleValidityDate })
                .Where(u => u.Role != "user").ToListAsync();

            List<User> expiratedUsers = new();

            foreach (User user in users)
            {
                if (user.RoleValidityDate is null)
                    throw new Exception("VRC: user.RoleValidityDate is null");

                DateTime dt = DateTime.Parse(user.RoleValidityDate);

                if (dt < DateTime.UtcNow)
                    expiratedUsers.Add(user);
            }

            if (expiratedUsers.Count > 0)
            {
                foreach (User user in expiratedUsers) //
                {
                    User? fullUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                    if (fullUser is null)
                        throw new Exception("ValidateChecker RoleValidityChecker.StartCheckAsync: fullUser is null");

                    fullUser.Role = "user";

                    context.Users.Update(fullUser);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}