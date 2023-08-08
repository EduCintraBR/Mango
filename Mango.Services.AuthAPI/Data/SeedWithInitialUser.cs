using Mango.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Data
{
    public class SeedWithInitialUser
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeedWithInitialUser(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedDataAsync()
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("ADMIN"))
            {
                await _roleManager.CreateAsync(new IdentityRole("ADMIN"));
            }
        }

        private async Task SeedAdminUserAsync()
        {
            if (await _userManager.FindByNameAsync("admin") == null)
            {
                var adminUser = new ApplicationUser
                {
                    Name = "Admin Padrão",
                    Email = "admin@mangodelivery.com",
                    NormalizedEmail = "ADMIN@MANGODELIVERY.COM",
                    UserName = "admin@mangodelivery.com",
                    NormalizedUserName = "ADMIN@MANGODELIVERY.COM",
                    PhoneNumber = "1234567890"
                };

                var result = await _userManager.CreateAsync(adminUser, "$Admin123*");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "ADMIN");
                }
            }
        }
    }
}
