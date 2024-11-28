using Application.Services.CacheService;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

public sealed class GroupMemberAddedDomainEventHandler : INotificationHandler<MemberAddedDomainEvent>
{
    private readonly ILogger<GroupMemberAddedDomainEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupMemberAddedDomainEventHandler(ILogger<GroupMemberAddedDomainEventHandler> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public Task Handle(MemberAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        GroupMembership groupMembership = notification.Membership;
        _logger.LogInformation("Member {Member} added to group {Group}", groupMembership.UserId, groupMembership.GroupId);
        _cacheService.SetStringAsync($"{groupMembership.UserId}-{groupMembership.GroupId}", groupMembership.GroupRoleId);
        return Task.CompletedTask;
    }
}

public sealed class GroupCreatedDomainEventHandler : INotificationHandler<GroupCreatedDomainEvent>
{
    private readonly ILogger<GroupCreatedDomainEventHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupCreatedDomainEventHandler(ILogger<GroupCreatedDomainEventHandler> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public Task Handle(GroupCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Group {Group} created by {User}", notification.Group.Id, notification.UserId);
        _cacheService.SetStringAsync($"{notification.UserId}-{notification.Group.Id}", GroupRoleIds.Admin);
        return Task.CompletedTask;
    }
}

public sealed class GroupMemberRemovedDomainEvenHandler : INotificationHandler<MemberRemovedDomainEvent>
{
    private readonly ILogger<GroupMemberRemovedDomainEvenHandler> _logger;
    private readonly ICacheService _cacheService;

    public GroupMemberRemovedDomainEvenHandler(ICacheService cacheService, ILogger<GroupMemberRemovedDomainEvenHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public Task Handle(MemberRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        GroupMembership groupMembership = notification.Membership;
        _logger.LogInformation("Member {Member} removed from group {Group}", groupMembership.UserId, groupMembership.GroupRoleId);
        _cacheService.RemoveAsync($"{groupMembership.UserId}-{groupMembership.GroupId}");
        return Task.CompletedTask;
    }
}
