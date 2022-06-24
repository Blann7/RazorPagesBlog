using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationContext context;
        private readonly DbLogger dbLogger;
        [BindProperty] public User SiteUser { get; set; } = new();

        public RegisterModel(ApplicationContext db, DbLogger dblogger)
        { 
            context = db; 
            this.dbLogger = dblogger;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            string confirmPassword = Request.Form["passwordagain"];
            if (confirmPassword is null || SiteUser.Password != confirmPassword)
            {
                return Page();
            }

            // To avoid repetition
            User? user = await context.Users
                .Select(u => new User { Name = u.Name })
                .FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if (user is not null) return Content("Пользователь с таким именем уже есть!", "text/html", Encoding.UTF8);

            user = await context.Users
                .Select(u => new User { Email = u.Email })
                .FirstOrDefaultAsync(u => u.Email == SiteUser.Email);
            if (user is not null) return Content("Пользователь с такой почтой уже есть!", "text/html", Encoding.UTF8);
            //--------------------

            SiteUser.Role = "user"; // Default Role for all site's users

            // Referral code
            if (Request.Cookies["bonus"] is not null)
            {
                bool success = await ReferralBonusHandler();
                if (success == false)
                {
                    Response.Cookies.Delete("bonus");
                    return BadRequest("Your bonus code invalid!");
                }
                else // send bonus for new user
                {
                    SiteUser.Money = Referral.NEWUSER_MONEY_BONUS;
                    SiteUser.Role = Referral.NEWUSER_ROLE_BONUS;
                    SiteUser.RoleValidityMs = DateTimeOffset.UtcNow.AddDays(Referral.NEWUSER_DAYROLE_BONUS)
                        .ToUnixTimeMilliseconds();

                    Response.Cookies.Delete("bonus");
                }
            }

            await context.Users.AddAsync(SiteUser);
            await context.SaveChangesAsync();

            // login --------------------------------------
            User? loginUser = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Name = u.Name })
                .FirstOrDefaultAsync(u => u.Name == SiteUser.Name);
            if(loginUser is null) return RedirectToPage("/auth/login");

            List<Claim> claims = new() {
                new Claim("Id", loginUser.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new (claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties authenticationProperties = new() { AllowRefresh = true }; // expire: session

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties);
            // --------------------------------------------

            dbLogger.Add(loginUser.Id, loginUser.Name!, LogViewer.Models.LogTypes.LogType.REGISTER_USER, "Зарегистрировался");

            return RedirectToPage("/index");
        }
        /// <returns>Returns TRUE if success, false - not success</returns>
        private async Task<bool> ReferralBonusHandler()
        {
            string code = Request.Cookies["bonus"]!;

            bool codeExist = await Referral.IsCodeAlreadyExist(code);
            if (!codeExist) return false; // if code not exist

            Referral? referral = await context.Referrals.AsNoTracking().FirstOrDefaultAsync(r => r.Code == code);
            if (referral is null) throw new Exception("RegisterPage: referral is null");

            User? referralUser = await context.Users.FirstOrDefaultAsync(u => u.Id == referral.UserId);
            if(referralUser is null) throw new Exception("RegisterPage: referralUser is null");

            referral.InvitedUsers++; // user invited, counter++;
            referralUser.Money += Referral.USER_MONEY_BONUS; // bonus for man, who invite user

            context.Referrals.Update(referral);
            context.Users.Update(referralUser);

            await context.SaveChangesAsync();
            return true;
        }
    }
}
