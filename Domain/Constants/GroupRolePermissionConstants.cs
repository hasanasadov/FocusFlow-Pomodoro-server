using System.Reflection;

namespace Application.Constants;

public static class GroupRolePermissionConstants
{
    private static Dictionary<string, bool> _permissions = [];

    public const string UpdateGroup = "UpdateGroup";
    public const string DeleteGroup = "DeleteGroup";
    public const string GetGroupMembers = "GetGroupMembers";

    public const string AddUserToGroup = "AddUserToGroup";
    public const string RemoveUserFromGroup = "RemoveUserFromGroup";
    public const string ChangeUserRoleInGroup = "ChangeUserRoleInGroup";

    public const string CreateGroupRole = "CreateGroupRole";
    public const string UpdateGroupRole = "UpdateGroupRole";
    public const string DeleteGroupRole = "DeleteGroupRole";

    public const string GroupRolePermissions = "GroupRolePermissions";
    public const string AllGroupRolePermissions = "AllGroupRolePermissions";

    public const string AddPermissionToGroupRole = "AddPermissionToGroupRole";
    public const string RemovePermissionFromGroupRole = "RemovePermissionFromGroupRole";

    public const string CreateProject = "CreateProject";
    public const string UpdateProject = "UpdateProject";
    public const string DeleteProject = "DeleteProject";

    public const string AddUserToProject = "AddUserToProject";
    public const string RemoveUserFromProject = "RemoveUserFromProject";

    public const string CreateTask = "CreateTask";
    public const string UpdateTask = "UpdateTask";
    public const string DeleteTask = "DeleteTask";
    public const string MarkTaskAsComplete = "MarkTaskAsComplete";
    public const string MarkTaskAsInComplete = "MarkTaskAsInComplete";

    public static Dictionary<string, bool> GetAllPermissions()
    {
        if (_permissions.Any())
        {
            return _permissions;
        }
        var fields =  typeof(GroupRolePermissionConstants).GetMembers(BindingFlags.Public | BindingFlags.Static);

        foreach (var field in fields)
        {
            _permissions.Add(field.Name.ToString(), true);
        }
        return _permissions;
    }
}