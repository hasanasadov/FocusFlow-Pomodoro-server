using Application.Features.Auths.Commands.GoogleLoginCommand;
using Application.Features.Auths.Commands.LoginCommand;
using Application.Features.Auths.Commands.RegisterCommand;
using Application.Services.AuthService.Local;

namespace WebAPI.Controllers;

public class AuthController(ILocalAuthService localAuthService) : BaseController
{
    private readonly ILocalAuthService _localAuthService = localAuthService;

    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterCommand request)
    {
        var result = await Mediator.Send(request);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok();
    }

    [HttpPost("signin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginCommand request)
    {
        var result = await Mediator.Send(request);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok();
    }

    [HttpHead]
    public async Task<IActionResult> Logout()
    {
        _ = await _localAuthService.Logout();
        return NoContent();
    }

    [HttpGet("google")]
    public async Task<IActionResult> GoogleLogin(string idToken)
    {
        await Mediator.Send(new GoogleLoginCommand(idToken));
        return Ok();
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult CurrentUser()
    {
        return Ok(HttpContext.User.Identity?.Name);
    }
}
