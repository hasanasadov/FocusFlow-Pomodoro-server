using Application.Constants;
using Domain.Events;

namespace Domain.Entities;

public sealed class GroupMembership : BaseEventEntity
{
    public required Guid UserId { get; set; }
    public int GroupId { get; set; }
    public required int GroupRoleId { get; set; } = (int) GroupRoleIds.User;

    public AppUser User { get; set; } = null!;
    public Group Group { get; set; } = null!;
    public GroupRole GroupRole { get; set; } = null!;

    public static GroupMembership Create(Guid userId, int groupId, int groupRoleId)
    {
        GroupMembership groupMembership = new()
        {
            UserId = userId,
            GroupId = groupId,
            GroupRoleId = groupRoleId
        };
        groupMembership.AddDomainEvent(new GroupMembershipAddedDomainEvent(groupMembership));
        return groupMembership;
    }

    public void ChangeRole(GroupRole role)
    {
        GroupRoleId = role.Id;
        this.GroupRole = role;
        AddDomainEvent(new GroupMembershipChangedDomainEvent(this));
    }
}