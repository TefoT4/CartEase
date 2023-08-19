using CartEase.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CartEase.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    [HttpGet]
    public IActionResult Items()
    {
        return Ok();
    }
    
    [HttpGet("{itemId}")]
    public IActionResult Items(int itemId)
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Post(CartItem item)
    {
        return Ok();
    }
    
    [HttpPut("{itemId}")]
    public IActionResult Put(int itemId, CartItem item)
    {
        return Ok();
    }

    [HttpDelete("{itemId}")]
    public IActionResult Delete(int itemId)
    {
        return Ok();
    }
    
    [HttpPost("{itemId}/image")]
    public IActionResult Image(int itemId, IFormFile imageFile)
    {
        return Ok();
    }
}