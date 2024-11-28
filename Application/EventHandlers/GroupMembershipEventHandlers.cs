
using Application.Services.CacheService;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

public sealed class GroupMembershipAddedDomainEventHandler : INotificationHandler<GroupMembershipAddedDomainEvent>
{
    private readonly ILogger<GroupMembershipAddedDomainEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupMembershipAddedDomainEventHandler(ILogger<GroupMembershipAddedDomainEventHandler> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public Task Handle(GroupMembershipAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        GroupMembership membership = notification.Membership;
        _logger.LogInformation("User {User} take Role {Role} in Group {Group}", membership.UserId, membership.GroupRoleId, membership.GroupId);
        _cacheService.SetStringAsync($"{membership.UserId}-{membership.GroupId}", membership.GroupRoleId);
        return Task.CompletedTask;
    }
}

public sealed class GroupMembershipChangedDomainEventtHandler : INotificationHandler<GroupMembershipChangedDomainEvent>
{
    private readonly ILogger<GroupMembershipChangedDomainEventtHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupMembershipChangedDomainEventtHandler(ILogger<GroupMembershipChangedDomainEventtHandler> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public Task Handle(GroupMembershipChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        GroupMembership membership = notification.Membership;
        _logger.LogInformation("User {User} take Role {Role} in Group {Group}", membership.UserId, membership.GroupRoleId, membership.GroupId);
        _cacheService.UpdateAsync($"{membership.UserId}-{membership.GroupId}", membership.GroupRoleId);
        return Task.CompletedTask;
    }
}