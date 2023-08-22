using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CartEase.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : Controller
{
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ILogger<AuthenticationController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("github-login")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult LoginWithGitHub()
    {
        try
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = "https://localhost:7029/swagger"
            };
            
            var authenticationSchemes =  new[] { "GitHub" };
            
            return Challenge(authenticationProperties, authenticationSchemes);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
    }
}