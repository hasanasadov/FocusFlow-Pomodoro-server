
namespace Application.DTOs.Errors;
public static class GroupRoleErrors
{
    public static ErrorDesc GroupRoleNotFound => new("Group role not found", "The group role does not exist.");
}

