using Microsoft.EntityFrameworkCore;
using BlogPhone.Models;
using BlogPhone.BackgroundServices.Messages;
using BlogPhone.BackgroundServices.Exceptions;
using BlogPhone.Models.Database;
using Microsoft.AspNetCore.Mvc;
using BlogPhone.Models.LogViewer;

namespace BlogPhone.BackgroundServices
{
    public class ValidateChecker : BackgroundService
    {
        DbLogger dbLogger = new();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ValidateRoleChecker(); // Checking role validity

                // -------------------------------------------------------------------------
                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task ValidateRoleChecker()
        {
            Info.Print("ValidateRoleChecker", $"UTC: {DateTimeOffset.UtcNow.ToString("G")}");

            using (ApplicationContext context = new ApplicationContext())
            {
                List<User> users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Role = u.Role, RoleValidityMs = u.RoleValidityMs })
                .Where(u => u.Role != "user").ToListAsync();

                List<User> expiratedUsers = new();

                foreach (User user in users)
                {
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
                            throw new VC_Exception("ValidateRoleChecker", "fullUser is null");

                        Info.Print("ValidateRoleChecker", $"Снят: (id {fullUser.Id}) {fullUser.Name}");
                        dbLogger.Add(fullUser.Id, fullUser.Name!, LogViewer.Models.LogTypes.LogType.EXPIRED_ROLE,
                            $"Истекла роль {fullUser.Role}");

                        fullUser.Role = "user"; // reset role to default

                        context.Users.Update(fullUser);
                        await context.SaveChangesAsync();
                    }
                }
            }
            Info.Print("________________________________________________");
        }
    }
}
