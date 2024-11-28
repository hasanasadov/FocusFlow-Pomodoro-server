using Application.Services.AuthService.Local;
using Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auths.Commands.RegisterCommand;

public sealed class RegisterCommandHandler(UserManager<AppUser> userManager, ILocalAuthService localAuthService) : IRequestHandler<RegisterCommand, Result>
{
    private readonly ILocalAuthService localAuthService = localAuthService;

    private UserManager<AppUser> UserManager { get; } = userManager;

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await localAuthService.RegisterAsync(new(request.Username, request.Email, request.Password));
    }
}

