using Application.Services.AuthService.Local;
using Application.Services.TokenService;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.AuthService.Local;

public sealed class LocalAuthService(UserManager<AppUser> userManager,
                                     ITokenService tokenService,
                                     IHttpContextAccessor httpContextAccessor,
                                     IConfiguration configuration) : ILocalAuthService
{
    private ITokenService TokenService = tokenService;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IConfiguration configuration = configuration;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext;

    public async Task<Result> LoginAsync(LoginDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.EmailOrUsername) ?? 
                   await userManager.FindByEmailAsync(request.EmailOrUsername);

        if (user == null)
        {
            return Result.Failure(Error.BadRequest(AuthErrors.CredentialsError));
        }

        var result = await userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
        {
            return Result.Failure(Error.BadRequest(AuthErrors.CredentialsError));
        }

        var roles = await userManager.GetRolesAsync(user);
        var tokenResult = await TokenService.GenerateToken(user, roles, cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure(tokenResult.Error);
        }
        return Result.Success();
    }

    public Task<Result> Logout()
    {
        string cookieName = configuration["TokenSettings:CookieName"]!;
        if (httpContext?.Request.Cookies[cookieName] != null)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                Path = "/"
            };
            httpContext.Response.Cookies.Append(cookieName, string.Empty, cookieOptions);
        }
        return Task.FromResult(Result.Success());
    }

    public async Task<Result> RegisterAsync(RegisterDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ??
                   await userManager.FindByNameAsync(request.Username);

        if (user != null)
        {
            return Result.Failure(Error.BadRequest(AuthErrors.UserAlreadyExists));
        }

        IdentityResult result = await userManager.CreateAsync(new()
        {
            UserName = request.Username,
            Email = request.Email
        }, request.Password);

        if (!result.Succeeded)
        {
            return Result.Failure(Error.BadRequest(new("One or more errors happened.", string.Join(",", result.Errors.Select(x => x.Description)))));
        }
        return Result.Success();
    }
}

