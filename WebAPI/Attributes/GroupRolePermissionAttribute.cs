using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class GroupRolePermissionAttribute : Attribute, IAuthorizationFilter
{
    public string[] Permissions { get; set; } = [];
    public bool CheckRoute { get; set; } = false;
    public int GroupId { get; private set; } = 0;
    public int ProjectId { get; private set; } = 0;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (CheckRoute)
        {
            var routeData = context.HttpContext.Request.RouteValues;
            if (routeData.TryGetValue("groupId", out var groupIdValue))
            {
                if (int.TryParse(groupIdValue?.ToString(), out int id))
                {
                    GroupId = id;
                }
            }
            if (routeData.TryGetValue("projectId", out var projectIdValue))
            {
                if (int.TryParse(projectIdValue?.ToString(), out int id))
                {
                    ProjectId = id;
                }
            }
        }

    }
}
