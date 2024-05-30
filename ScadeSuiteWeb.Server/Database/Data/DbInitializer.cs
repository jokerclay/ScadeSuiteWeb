using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using ScadeSuiteWeb.Server.Models.User;
using ScadeSuiteWeb.Shared.Utils;

namespace ScadeSuiteWeb.Server.Database.Data;


public class DbInitializer(UserManager<XUser> userManager, RoleManager<XRole> roleManager)
{
    public async Task InitializeAsync(AppDbContext context)
    {
        // Ensure roles are created
        await EnsureRoleCreatedAsync("Administrator", "管理员");
        await EnsureRoleCreatedAsync("Engineer", "工程师");

        string adminPassword =  Encrypt.Sha256EncryptString("adminadmin");
        string testengineerPassword =  Encrypt.Sha256EncryptString("123123");
        // Ensure users are created and assigned roles
        await EnsureUserCreatedAsync("testengineer@example.com", "testengineer", "Engineer", testengineerPassword);
        await EnsureUserCreatedAsync("admin@example.com", "admin", "Administrator", adminPassword );
    }

    private async Task EnsureRoleCreatedAsync(string roleName, string normalizedRoleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new XRole
            {
                Name = roleName,
                NormalizedName = normalizedRoleName,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            await roleManager.CreateAsync(role);
        }
    }

    private async Task EnsureUserCreatedAsync(string email, string userName, string roleName, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new XUser { UserName = userName, Email = email, SecurityStamp = Guid.NewGuid().ToString() };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
            else
            {
                // Handle user creation failure
                // throw new Exception($"Failed to create user {userName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            // Ensure the user is assigned the role if they already exist
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }   
        
        /*
        var  testengineer = await userManager.FindByNameAsync("testengineer");
        var  admin = await userManager.FindByNameAsync("admin");
        if (admin != null)
        {
            var res = userManager.GetRolesAsync(admin);
            await userManager.AddToRoleAsync(admin , "Engineer");
            var aa = userManager.GetRolesAsync(admin);
        }
        Console.WriteLine( "testengineer-->" + JsonSerializer.Serialize(testengineer ));
        Console.WriteLine("admin-->" + JsonSerializer.Serialize(admin));
    */
        
        
        
    }
}
