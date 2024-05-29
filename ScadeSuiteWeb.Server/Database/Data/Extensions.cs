using Microsoft.AspNetCore.Identity;
using ScadeSuiteWeb.Server.Models.User;

namespace ScadeSuiteWeb.Server.Database.Data;

public static class Extensions
{
    
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<XUser>>();
        var roleManager = services.GetRequiredService<RoleManager<XRole>>();

        context.Database.EnsureCreated();




        context.Database.EnsureCreated();

        // Use DbInitializer to seed data into the database
        var initializer = new DbInitializer(userManager, roleManager);
        initializer.InitializeAsync(context).GetAwaiter().GetResult();
    }
    
}
