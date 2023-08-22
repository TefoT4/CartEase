using System.Security.Claims;
using CartEase.Application.Domain;
using CartEase.Application.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace CartEase.Api.Controllers;

public class BaseController : ControllerBase
{
    private string currentUserId;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseController(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetUserId()
    {
        if (_httpContextAccessor.HttpContext.User == null)
        {
            return string.Empty;
        }
        
        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;

            var enumerable = claims as Claim[] ?? claims.ToArray();
            var username = enumerable.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            
            if (! _userService.ExistAsync(username).Result)
            {
                var names = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value?.Split(' ');

                // create a user from claims
                var user = new User()
                {
                    Email = enumerable.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    Username = username,
                    FirstName = names?.FirstOrDefault(),
                    LastName = names?.LastOrDefault(),
                    AuthProviderId = enumerable.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value
                };

                var result = _userService.CreateAsync(user).Result;

                if (result.IsSuccessful)
                    return result.Data.AuthProviderId;
            }
            else
            {
                currentUserId = enumerable.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                return currentUserId;
            }
                
        }

        return string.Empty;
    }

    protected string? CurrentUserId => Convert.ToString(!string.IsNullOrEmpty(currentUserId) ? currentUserId : GetUserId());
}