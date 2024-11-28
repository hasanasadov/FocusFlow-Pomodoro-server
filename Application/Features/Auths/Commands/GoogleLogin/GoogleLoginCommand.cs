namespace Application.Features.Auths.Commands.GoogleLoginCommand;

public sealed record GoogleLoginCommand(string IdToken) : IRequest;

