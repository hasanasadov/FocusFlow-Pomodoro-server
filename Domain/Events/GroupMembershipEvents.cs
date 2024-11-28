using Domain.Entities;

namespace Domain.Events;

public sealed record GroupMembershipAddedDomainEvent(GroupMembership Membership) : IDomainEvent;

public sealed record GroupMembershipChangedDomainEvent(GroupMembership Membership) : IDomainEvent;
