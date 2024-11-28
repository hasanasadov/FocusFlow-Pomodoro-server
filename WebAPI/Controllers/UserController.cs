using Application.Services.AuthService.Local;

namespace WebAPI.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILocalAuthService _authService;

    public UserController(IUserService userService, ILocalAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }


    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUserDataAsync();
        return Ok(users);
    }

    [HttpPatch("upload-image")]
    public IActionResult UpdateUser(IFormFile file)
    {
        _userService.UploadUserProfileAsync(file);
        return Ok();
    }
}
