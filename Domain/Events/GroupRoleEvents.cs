using Domain.Entities;

namespace Domain.Events;

public sealed record GroupRoleCreatedDomainEvent(GroupRole GroupRole) : IDomainEvent;

public sealed record GroupRoleUpdatedDomainEvent(GroupRole GroupRole) : IDomainEvent;

public sealed record GroupRoleDeletedDomainEvent(GroupRole GroupRole) : IDomainEvent;
