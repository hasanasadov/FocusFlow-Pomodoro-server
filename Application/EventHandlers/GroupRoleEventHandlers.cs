using Application.Services.CacheService;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

public sealed class GroupRoleCreatedDomainEventHandler : INotificationHandler<GroupRoleCreatedDomainEvent>
{
    private readonly ILogger<GroupRoleCreatedDomainEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupRoleCreatedDomainEventHandler(ILogger<GroupRoleCreatedDomainEventHandler> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public Task Handle(GroupRoleCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        GroupRole groupRole = notification.GroupRole;
        _logger.LogInformation("Group Role {Role} created", groupRole.Name);
        _cacheService.SetStringAsync($"{groupRole.Id}-pms", groupRole.Permissions);
        return Task.CompletedTask;
    }
}

public sealed class GroupRoleUpdatedDomainEventHandler : INotificationHandler<GroupRoleUpdatedDomainEvent>
{
    private readonly ILogger<GroupRoleUpdatedDomainEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupRoleUpdatedDomainEventHandler(ILogger<GroupRoleUpdatedDomainEventHandler> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public Task Handle(GroupRoleUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        GroupRole groupRole = notification.GroupRole;
        _logger.LogInformation("Group Role {Role} updated", groupRole.Name);
        _cacheService.UpdateAsync($"{groupRole.Id}-pms", groupRole.Permissions);
        return Task.CompletedTask;
    }
}

public sealed class GroupRoleDeletedDomainEventHandler : INotificationHandler<GroupRoleDeletedDomainEvent>
{
    private readonly ILogger<GroupRoleDeletedDomainEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupRoleDeletedDomainEventHandler(ILogger<GroupRoleDeletedDomainEventHandler> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public Task Handle(GroupRoleDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        GroupRole groupRole = notification.GroupRole;
        _logger.LogInformation("Group Role {Role} deleted", groupRole.Name);
        _cacheService.RemoveAsync($"{groupRole.Id}-pms");
        return Task.CompletedTask;
    }
}
