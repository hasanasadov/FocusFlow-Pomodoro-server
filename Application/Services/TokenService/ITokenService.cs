using Domain.Entities.Auth;
using System.Security.Claims;

namespace Application.Services.TokenService;

public interface ITokenService
{
    Task<Result<string>> GenerateToken(AppUser user, IList<string> roles, CancellationToken cancellationToken);
    Task<Result<string>> GenerateRefreshToken(AppUser user, CancellationToken cancellationToken);
    Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    Task<Result<string>> GenerateTokenFromRefreshToken(string refreshToken, CancellationToken cancellationToken);
}
