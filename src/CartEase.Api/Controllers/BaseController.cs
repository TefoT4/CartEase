using CartEase.Application.Domain;
using CartEase.Application.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace CartEase.Api.Controllers;

public class BaseController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseController(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<string> GetUserId()
    {
        if (_httpContextAccessor.HttpContext.User == null)
        {
            return string.Empty;
        }
        
        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            if (! await _userService.ExistAsync(_httpContextAccessor.HttpContext.User.Identity.Name))
            {
                // create a user from claims
                var user = new User()
                {
                    Email = "",
                    Username = _httpContextAccessor.HttpContext.User.Identity.Name,
                    FirstName = "",
                    LastName = "",
                    AuthProviderId = ""
                };

                var result = await _userService.CreateAsync(user);

                if (result.IsSuccessful)
                    return result.Data.AuthProviderId;
            }
        }

        return string.Empty;
    }

    protected string CurrentUserId => GetUserId().Result;
}