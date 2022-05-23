using BlogPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace ValidateChecker
{
    internal class RoleValidityChecker
    {
        ApplicationContext context;
        public RoleValidityChecker()
        {
            context = new ApplicationContext(new DbContextOptions<ApplicationContext>());
        }
        internal async void StartCheckAsync()
        {
            List<User> users = await context.Users.AsNoTracking()
                .Select(u => new User { Id = u.Id, Role = u.Role, RoleValidityDate = u.RoleValidityDate })
                .Where(u => u.Role != "user").ToListAsync();

            List<User> expiratedUsers = new();

            foreach (User user in users)
            {
                if (user.RoleValidityDate is null)
                    throw new Exception("ValidateChecker RoleValidityChecker.StartCheckAsync: user.RoleValidityDate is null");

                DateTime dt = DateTime.Parse(user.RoleValidityDate);

                if(dt < DateTime.UtcNow)
                    expiratedUsers.Add(user);
            }

            if(expiratedUsers.Count > 0)
            {
                foreach (User user in expiratedUsers) //
                {
                    User? fullUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                    if(fullUser is null)
                        throw new Exception("ValidateChecker RoleValidityChecker.StartCheckAsync: fullUser is null");

                    fullUser.Role = "user";
                    Console.WriteLine($"Отработал: (id {fullUser.Id}) {fullUser.Name}");

                    context.Users.Update(fullUser);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
