namespace Application.Services.AuthService;

public interface IAuthService
{
    Task AuthenticateAsync(GoogleAuthenticateDto request);
}

public sealed record GoogleAuthenticateDto(string IdToken);