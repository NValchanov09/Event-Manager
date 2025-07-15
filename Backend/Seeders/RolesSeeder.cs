using Microsoft.AspNetCore.Identity;

namespace EventManagerBackend.Seeders
{
    public static class RolesSeeder
    {
        private static readonly string[] Roles = new[] { "Administrator", "User" };

        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
