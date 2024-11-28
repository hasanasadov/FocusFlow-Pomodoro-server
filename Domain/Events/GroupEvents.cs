using Domain.Entities;

namespace Domain.Events;

public sealed record MemberAddedDomainEvent(GroupMembership Membership) : IDomainEvent;
public sealed record GroupCreatedDomainEvent(Group Group, string UserId) : IDomainEvent;
public sealed record MemberRemovedDomainEvent(GroupMembership Membership) : IDomainEvent;