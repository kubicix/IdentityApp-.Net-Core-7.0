using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "admin";
        private const string adminPassword = "Admin_123";

        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();

            // Ensure the database is created and all migrations are applied
            context.Database.Migrate();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(adminUser);

            if (user == null)
            {
                user = new AppUser
                {
                    FullName = "Kubilay Birer",
                    UserName = adminUser,
                    Email = "admin@kb.com",
                    PhoneNumber = "44444444"
                };

                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}
