using Microsoft.AspNetCore.Identity;
using OnlineCurriculum.Enums;

namespace OnlineCurriculum.Extensions;

public static class ConfigureRolesExtension
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in Enum.GetNames(typeof(Role)))
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}