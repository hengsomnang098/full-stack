using FullStack.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Infrastructure.Data;

public static class DbSeeder
{
    public const string AdminRole = "Admin";

    public static async Task InitializeAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var mode = config["Database:InitMode"] ?? "EnsureCreated";
        var db = services.GetRequiredService<AppDbContext>();

        switch (mode.ToLowerInvariant())
        {
            case "migrate":
                await db.Database.MigrateAsync();
                break;
            case "ensurecreated":
                await db.Database.EnsureCreatedAsync();
                break;
        }

        if (!config.GetValue("Seed:Enabled", true)) return;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            var r = await roleManager.CreateAsync(new IdentityRole(AdminRole));
            if (!r.Succeeded)
                logger.LogError("Failed to create role {Role}: {Errors}", AdminRole, string.Join(", ", r.Errors.Select(e => e.Description)));
        }

        var tel = config["Seed:AdminTel"] ?? "099998888";
        var password = config["Seed:AdminPassword"] ?? "Admin@12345";

        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Tel == tel);
        if (employee is null)
        {
            employee = new Employee
            {
                Firstname = "Admin",
                Lastname = "System",
                Gender = true,
                Tel = tel,
                Email = config["Seed:AdminEmail"] ?? "admin@example.com",
                Status = true,
                CreateAt = DateTime.UtcNow
            };
            db.Employees.Add(employee);
            await db.SaveChangesAsync();
        }

        var admin = await userManager.Users.FirstOrDefaultAsync(u => u.Tel == tel);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = tel,
                Tel = tel,
                FirstName = "Admin",
                LastName = "System",
                Gender = true,
                Status = true,
                Email = config["Seed:AdminEmail"] ?? "admin@example.com",
                EmailConfirmed = true,
                EmployeeId = employee.Id
            };
            var create = await userManager.CreateAsync(admin, password);
            if (!create.Succeeded)
            {
                logger.LogError("Failed to seed admin: {Errors}", string.Join(", ", create.Errors.Select(e => e.Description)));
                return;
            }
            logger.LogInformation("Seeded admin user with tel {Tel}", tel);
        }
        else if (admin.EmployeeId != employee.Id)
        {
            admin.EmployeeId = employee.Id;
            await userManager.UpdateAsync(admin);
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
            await userManager.AddToRoleAsync(admin, AdminRole);
    }
}
