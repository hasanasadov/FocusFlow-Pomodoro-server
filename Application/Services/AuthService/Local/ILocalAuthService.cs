namespace Application.Services.AuthService.Local;

public interface ILocalAuthService
{
    Task<Result> LoginAsync(LoginDto request, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(RegisterDto request);
    Task<Result> Logout();
}

