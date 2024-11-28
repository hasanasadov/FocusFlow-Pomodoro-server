
namespace Application.DTOs.Errors;
public static class GroupErrors
{
    public static ErrorDesc UserAlreadyInGroup => new("User already in group", "The user is already a member of the group.");
    public static ErrorDesc GroupNotFound => new("Group not found", "The group does not exist.");
    public static ErrorDesc UserNotInGroup => new("User not in group", "The user is not a member of the group.");
    public static ErrorDesc RoleNotFound => new("Role not found", "The role does not exist.");

}
