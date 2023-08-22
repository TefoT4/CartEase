using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

public class YourController : ControllerBase
{
    private readonly ILogger<YourController> _logger;
    private readonly IConfiguration _configuration;

    public YourController(ILogger<YourController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("github-login")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult LoginWithGitHub()
    {
        try
        {
            string redirectUri = _configuration.GetValue<string>("AuthSettings:RedirectUri");

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = string.IsNullOrWhiteSpace(redirectUri)
                              ? "https://localhost:7029/swagger"
                              : redirectUri
            };

            var authenticationSchemes = new[] { "GitHub" };

            return Challenge(authenticationProperties, authenticationSchemes);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
    }
}