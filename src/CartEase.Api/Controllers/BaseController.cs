using CartEase.Application.Service;
using CartEase.Application.Validators;
using Microsoft.AspNetCore.Mvc;

namespace CartEase.Api.Controllers;

public class BaseController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
    }

    public string? CurrentUserId => GetUserId();
}