using Microsoft.AspNetCore.Identity;

namespace He_SheStore.Areas.Identity.Data
{
    public static class ContextSeed
    {

        public static async Task seedRolesAsync(RoleManager<IdentityRole> roleManager)
        {

            string[] roleNames = { "SuperAdmin", "User", "GuestUser" };

            foreach (var roleName in roleNames)
            {
                // Check if the role already exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Create the role if it doesn't exist
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultAdmin = new ApplicationUser
            {
                UserName = "SuperAdmin123@gmail.com",
                Email = "SuperAdmin123@gmail.com",
                EmailConfirmed = true,
                FirstName = "Web site",
                LastName= "Owner"
            };
            if (userManager.Users.All(u => u.Id != defaultAdmin.Id))
            {
                var user = await userManager.CreateAsync(defaultAdmin, "SuperAdmin123@");

                if (user.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, "SuperAdmin");

                }
            }

        }

    }

}
