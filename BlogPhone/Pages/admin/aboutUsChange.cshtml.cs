using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BlogPhone.Pages.admin
{
    [Authorize]
    public class aboutUsChangeModel : PageModel
    {
        readonly ApplicationContext context;
        [BindProperty] public AboutUsInfo? AUI { get; set; }
        public AboutUsInfo? oldAUI { get; set; }
        [BindProperty] public IFormFile? Image1 { get; set; }
        [BindProperty] public IFormFile? Image2 { get; set; }
        public User? SiteUser { get; set; }
        public aboutUsChangeModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            (bool, bool) getInfoResult = await TryGetSiteUserAsync();
            if (getInfoResult != (true, true)) return BadRequest();

            bool access = AccessChecker.RoleCheck(SiteUser!.Role, "admin", "moder");
            if (!access) return RedirectToPage("/auth/logout");

            AUI = await context.AboutUsPage.FirstOrDefaultAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            oldAUI = await context.AboutUsPage.FirstOrDefaultAsync();
            if (oldAUI is null) return BadRequest();
            if (AUI is null) return RedirectToPage("/admin/aboutUsChange");

            if (Image1 is not null) AUI.P1_ImageData = ImageWorking.GetImageDataFromIFormFile(Image1);
            else AUI.P1_ImageData = oldAUI.P1_ImageData;
            if (Image2 is not null) AUI.P2_ImageData = ImageWorking.GetImageDataFromIFormFile(Image2);
            else AUI.P2_ImageData = oldAUI.P2_ImageData;

            if (AUI?.P1_Title is null || AUI?.P1_Text is null || AUI?.P2_Title is null || AUI?.P2_Text is null) 
                return RedirectToPage("/admin/aboutUsChange");

            oldAUI.P1_ImageData = AUI.P1_ImageData;
            oldAUI.P2_ImageData = AUI.P2_ImageData;
            oldAUI.P1_Title = AUI.P1_Title;
            oldAUI.P2_Title = AUI.P2_Title;
            oldAUI.P1_Text = AUI.P1_Text;
            oldAUI.P2_Text = AUI.P2_Text;

            context.AboutUsPage.Update(oldAUI);
            await context.SaveChangesAsync();

            return RedirectToPage("/admin/aboutUsChange");
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
                .Select(u => new User { Id = u.Id, Email = u.Email, Role = u.Role })
                .FirstOrDefaultAsync(u => u.Id.ToString() == idString);
            if (SiteUser is null) return (true, false);

            ViewData["email"] = SiteUser.Email;

            return (true, true);
        }
    }
}
