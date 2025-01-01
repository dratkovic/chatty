using Chatty.Authentication.Api.Domain;
using Microsoft.AspNetCore.Identity;

namespace Chatty.Authentication.Api.Infrastructure;

public static class Seeder
{
    public static async Task Seed(this IHost app, bool isDevEnvironment)
    {
        using var scope = app.Services.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();

        await Seed(dbContext, roleManager, userManager, isDevEnvironment);
    }
    
    public static async Task Seed(AuthenticationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, bool isDevEnvironment)
    {
        await dbContext.Database.EnsureCreatedAsync();
    
        await SeedRolesAsync(roleManager);
        await SeedUserAsync(userManager, isDevEnvironment);
    }
    
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(Roles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
        }
    }
    
    private static async Task SeedUserAsync(UserManager<AppUser> userManager, bool isDevelopment)
    {
        if (isDevelopment)
        {
            var adminUser = await userManager.FindByEmailAsync(SeedUsers.Admin.Email);
            if(adminUser is null)
            {
                var admin = new AppUser
                {
                    UserName = SeedUsers.Admin.UserName,
                    Email = SeedUsers.Admin.Email,
                    EmailConfirmed = true,
                    FirstName = SeedUsers.Admin.FirstName,
                    LastName = SeedUsers.Admin.LastName
                };
                var createAdminResult = await userManager.CreateAsync(admin, SeedUsers.Admin.Password);

                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
                }
            }
            
            var user1 = await userManager.FindByEmailAsync(SeedUsers.User1.Email);

            if (user1 is null)
            {
                var user = new AppUser
                {
                    UserName = SeedUsers.User1.UserName,
                    Email = SeedUsers.User1.Email,
                    EmailConfirmed = true,
                    FirstName = SeedUsers.User1.FirstName,
                    LastName = SeedUsers.User1.LastName
                };
                var createShrederResult = await userManager.CreateAsync(user, SeedUsers.User1.Password);

                if (createShrederResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.User);
                }
            }
            
            var user2 = await userManager.FindByEmailAsync(SeedUsers.User2.Email);
            
            if (user2 is null)
            {
                var user = new AppUser
                {
                    UserName = SeedUsers.User2.UserName,
                    Email = SeedUsers.User2.Email,
                    EmailConfirmed = true,
                    FirstName = SeedUsers.User2.FirstName,
                    LastName = SeedUsers.User2.LastName
                };
                var createShrederResult = await userManager.CreateAsync(user, SeedUsers.User2.Password);

                if (createShrederResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.User);
                }
            }
        }
    }

    private static class SeedUsers
    {
        internal static class Admin
        {
            public static string UserName => "admin@chatty.com";
            public static string Email => "admin@chatty.com";
            public static string Password => "Admin@123";
            public static string FirstName => "Admin";
            public static string LastName => "McAdmin";
        }
        
        internal static class User1
        {
            public static string UserName => "user1@chatty.com";
            public static string Email => "user1@chatty.com";
            public static string Password => "User1@123";
            public static string FirstName => "User1";
            public static string LastName => "McUser1";
        }
        
        internal static class User2
        {
            public static string UserName => "user2@chatty.com";
            public static string Email => "user2@chatty.com";
            public static string Password => "User2@123";
            public static string FirstName => "User2";
            public static string LastName => "McUser2";
        }
    }
}