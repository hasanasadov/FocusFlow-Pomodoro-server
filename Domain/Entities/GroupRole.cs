using Application.Constants;
using Domain.Events;

namespace Domain.Entities;

public sealed class GroupRole : BaseEventEntity
{
    public required string Name { get; set; }
    public bool IsDefault { get; set; } = false;
    public int? GroupId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Group Group { get; set; } = default;

    public List<string> Permissions { get; set; } = [];

    public static GroupRole Create(string name, int? groupId, List<string> permissions)
    {
        var groupRole = new GroupRole
        {
            Name = name,
            GroupId = groupId,
            IsDefault = false
        };
        foreach (string permission in permissions)
        {
            GroupRolePermissionConstants.GetAllPermissions().TryGetValue(permission, out bool isExist);
            if (isExist)
            {
                groupRole.Permissions.Add(permission);
            }
        }
        groupRole.AddDomainEvent(new GroupRoleCreatedDomainEvent(groupRole));
        return groupRole;
    }

    public void Delete()
    {
        IsDeleted = true;
        AddDomainEvent(new GroupRoleDeletedDomainEvent(this));
    }

    public void Update(string name, List<string> permissions)
    {
        Name = name;
        ChangePermission(permissions);
    }

    private void ChangePermission(List<string> permissions)
    {
        Permissions.Clear();
        AddPermission(permissions);
        AddDomainEvent(new GroupRoleUpdatedDomainEvent(this));
    }

    public void AddPermission(List<string> permissions)
    {
        foreach (string permission in permissions)
        {
            GroupRolePermissionConstants.GetAllPermissions().TryGetValue(permission, out bool isExist);
            if (!Permissions.Contains(permission) && isExist)
            {
                Permissions.Add(permission);
            }
        }
        AddDomainEvent(new GroupRoleUpdatedDomainEvent(this));
    }

    public void RemovePermission(List<string> permissions)
    {
        foreach (string permission in permissions)
        {
            if (Permissions.Contains(permission))
            {
                Permissions.Remove(permission);
            }
        }
        AddDomainEvent(new GroupRoleUpdatedDomainEvent(this));
    }


}