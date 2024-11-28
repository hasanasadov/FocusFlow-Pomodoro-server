namespace WebAPI.Controllers;
[Route("")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok($"Focus Flow environment is {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
    }
}
