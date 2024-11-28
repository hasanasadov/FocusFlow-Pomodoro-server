using Application.Constants;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Context;

public class AppDbContextInitializer(
    AppDbContext Context,
    ILogger<AppDbContextInitializer> Logger,
    UserManager<AppUser> UserManager
    )
{
    private readonly AppDbContext _context = Context;
    private readonly ILogger<AppDbContextInitializer> _logger = Logger;
    private readonly UserManager<AppUser> _userManager = UserManager;

    public async Task InitialiseAsync()
    {
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Count() != 0)
            {
                await _context.Database.MigrateAsync();
            }
            if (!await _context.Users.AnyAsync())
            {
                await SeedUserAsync();
            }
            if (!await _context.GroupRoles.AnyAsync())
            {
                await SeedGroupRolesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    private async Task SeedUserAsync()
    {
        var user = new AppUser
        {
            UserName = "elcaneyvazli",
            Email = "elcaneyvazli77@gmail.com"
        };
        var result = await _userManager.CreateAsync(user, "067698965236");

        user = new AppUser
        {
            UserName = "a",
            Email = "rehh213@gmail.com"
        };
        result = await _userManager.CreateAsync(user, "a");

    }

    private async Task SeedGroupRolesAsync()
    {
        _context.GroupRoles.FromSqlRaw("SET IDENTITY_INSERT dbo.GroupRoles ON");
        var roles = new List<GroupRole>
        {
            new GroupRole { 
                Id = 1,
                Name = "Admin", 
                IsDefault = true, 
                Permissions = GroupRolePermissionConstants.GetAllPermissions().Select(x => x.Key).ToList()
            },
            new GroupRole { 
                Id = 2,
                Name = "User", 
                IsDefault = true, 
                Permissions = [] 
            }
        };
        await _context.GroupRoles.AddRangeAsync(roles);
        _context.GroupRoles.FromSqlRaw("SET IDENTITY_INSERT dbo.GroupRoles OFF");
        await _context.SaveChangesAsync();
    }
}
