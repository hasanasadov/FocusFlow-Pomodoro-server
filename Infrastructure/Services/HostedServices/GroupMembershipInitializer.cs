using Application.Services.CacheService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.HostedServices;

public class GroupMembershipInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public GroupMembershipInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            var groupMemberships = await dbContext.GroupMemberships
                                        .AsNoTracking()
                                        .Include(x => x.GroupRole)
                                        .ToListAsync(cancellationToken);

            var request = groupMemberships.Select(x => new GroupMembershipInitializerRequest(x.GroupId, x.UserId.ToString(), x.GroupRoleId)).ToList();


            await cacheService.ClearAsync();
            foreach (var membership in request)
            {
                await cacheService.SetStringAsync($"{membership.UserId}-{membership.GroupId}", membership.RoleId);
            }

            var groupRoles = await dbContext.GroupRoles.AsNoTracking().ToListAsync();
            foreach (var group in groupRoles)
            {
                await cacheService.SetStringAsync($"{group.Id}-pms", group.Permissions);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public sealed record GroupMembershipInitializerRequest(int GroupId, string UserId, int RoleId);