namespace WebAPI.Handlers;

using Application.Services.AuthService.Local;
using Microsoft.AspNetCore.Authorization;

public class UserExistsRequirement : IAuthorizationRequirement
{
    public UserExistsRequirement()
    {
    }
}

public class UserExistsHandler : AuthorizationHandler<UserExistsRequirement>
{
    private readonly IUserService _userService;
    private readonly ILocalAuthService _localAuthService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserExistsHandler(IUserService userService, IHttpContextAccessor httpContextAccessor, ILocalAuthService localAuthService)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _localAuthService = localAuthService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserExistsRequirement requirement)
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var isExist = await _userService.UserExists(userId);
        if (userId is not null && isExist)
        {
            context.Succeed(requirement);
        }
        else
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                _ = await _localAuthService.Logout();
                context.Fail();
            }
        }
    }
}