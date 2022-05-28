using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;

namespace BlogPhone.Pages
{
    public class IndexModel : PageModel
    {
        readonly ApplicationContext context;
        private Mutex mutex = new Mutex();
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
                if (HttpContext.User.FindFirst("Id") is null) return RedirectToPage("/auth/logout");
                int Id = int.Parse(HttpContext.User.FindFirst("Id")!.Value);

                SiteUser = await context.Users.AsNoTracking()
                    .Select(u => new User { Id = u.Id, Email = u.Email, BanDate = u.BanDate })
                    .FirstOrDefaultAsync(u => u.Id == Id);
                if(SiteUser is null) return RedirectToPage("/auth/logout");

                // ban check
                bool banned = AccessChecker.BanCheck(SiteUser.BanDate);
                if (!banned) return Content("You banned on this server, send on this email: " + AccessChecker.EMAIL);
                // ---------

                IsAuthorize = HttpContext.User.Identity.IsAuthenticated;

                await VRCAsync(); // Validate Roles Checker
            }

            if (Request.Cookies["indexLoad"] is null)
            {
                Response.Cookies.Append("indexLoad", "3");
                return RedirectToPage("/index");
            }

            List<ArticleBlog> arts = await context.ArticleBlogs.AsNoTracking().ToListAsync();
            if (Request.Cookies["indexLoad"] == "all") Articles = arts;
            else Articles = arts.TakeLast(int.Parse(Request.Cookies["indexLoad"]!)).ToList();

            return Page();
        }
        public IActionResult OnPostMore()
        {
            if (Request.Cookies["indexLoad"] == "3") Response.Cookies.Append("indexLoad", "10");
            else if (Request.Cookies["indexLoad"] == "10") Response.Cookies.Append("indexLoad", "20");
            else if (Request.Cookies["indexLoad"] == "20") Response.Cookies.Append("indexLoad", "30");
            else Response.Cookies.Append("indexLoad", "all");

            return RedirectToPage("/index");
        }
        public IActionResult OnPostLess()
        {
            Response.Cookies.Append("indexLoad", "3");

            return RedirectToPage("/index");
        }
        // ValidateRoleChecker -----------------------------------------------------------------------------------------------------
        public async Task VRCAsync()
        {
            //mutex.WaitOne(); // locker
            List<User> users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Role = u.Role, RoleValidityDate = u.RoleValidityDate })
                .Where(u => u.Role != "user").ToListAsync();

            List<User> expiratedUsers = new();

            foreach (User user in users)
            {
                if (user.RoleValidityDate is null)
                    throw new Exception("ValidateChecker RoleValidityChecker.StartCheckAsync: user.RoleValidityDate is null");

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
                    Console.WriteLine($"Отработал: (id {fullUser.Id}) {fullUser.Name}");

                    context.Users.Update(fullUser);
                    await context.SaveChangesAsync();
                }
            }
            //mutex.ReleaseMutex(); // release locker
        }
    }
}