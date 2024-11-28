using Application.Services.AuthService;

namespace Application.Features.Auths.Commands.GoogleLoginCommand;

public sealed class GoogleLoginCommandHandler(IAuthService authService) : IRequestHandler<GoogleLoginCommand>
{
    public IAuthService AuthService { get; } = authService;

    public async Task Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        await AuthService.AuthenticateAsync(new(request.IdToken));
    }
}
