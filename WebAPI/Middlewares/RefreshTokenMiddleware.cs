using Application.Services.TokenService;
using Infrastructure.Services.TokenService;
using Microsoft.Extensions.Options;
using System.Net;

namespace WebAPI.Middlewares;

public sealed class RefreshTokenMiddleware(RequestDelegate Next, IOptions<TokenSetting> TokenSettings)
{
    private readonly RequestDelegate _next = Next;
    private readonly TokenSetting _tokenSettings = TokenSettings.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            var refreshToken = context.Request.Cookies[_tokenSettings.RefreshCookieName];
            if (refreshToken is not null)
            {
                var tokenService = context.RequestServices.GetRequiredService<ITokenService>();
                var tokenResult = await tokenService.GenerateTokenFromRefreshToken(refreshToken, CancellationToken.None);

                if (tokenResult.IsSuccess)
                {
                    context.Request.Headers["Authorization"] = $"Bearer {tokenResult.Value}";
                    await _next(context);
                }
            }
        }
    }
}

