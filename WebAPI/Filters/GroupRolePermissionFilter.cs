using Application.Configurations;
using Application.Services.CacheService;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.Attributes;

namespace WebAPI.Filters;

public sealed class GroupRolePermissionFilter : IAsyncActionFilter
{
    private readonly ICacheService _cache;
    private readonly IUserContext _userContext;
    public GroupRolePermissionFilter(ICacheService cache, IUserContext userContext)
    {
        _cache = cache;
        _userContext = userContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var attribute = context.ActionDescriptor.EndpointMetadata
            .OfType<GroupRolePermissionAttribute>().FirstOrDefault();
        
        if (attribute == null || !_userContext.IsAuthenticated)
        {
            await next();
            return;
        }

        string key = $"{_userContext.UserId}-{attribute.GroupId}";
        string? cachedPermissions = await _cache.GetStringAsync<string>(key);
        
        if (string.IsNullOrEmpty(cachedPermissions))
        {
            context.Result = new ForbidResult();
            return;
        }

        string[] userPermissions = await _cache.GetStringAsync<string[]>(cachedPermissions + "-pms") ?? [];
        
        if (userPermissions == null || !userPermissions.Any())
        {
            context.Result = new ForbidResult();
            return;
        }

        if (attribute.Permissions.Length == 0 || userPermissions.Intersect(attribute.Permissions).Any())
        {
            await next();
            return;
        }

        context.Result = new ForbidResult();
    }
}
