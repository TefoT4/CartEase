using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CartEase.Api.Controllers;

public class AuthenticationController : Controller
{
    [HttpGet("github-login")]
    public IActionResult LoginWithGitHub()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/swagger/index.html"
        }, "GitHub");
    }
}