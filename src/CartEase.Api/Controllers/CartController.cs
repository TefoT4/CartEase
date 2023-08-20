using CartEase.Application.Domain;
using CartEase.Application.Service;
using CartEase.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartEase.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CartController : BaseController
{
    private readonly ICartService _cartService;
    private readonly CartItemValidator _cartItemValidator;

    public CartController(IHttpContextAccessor httpContextAccessor, ICartService cartService, CartItemValidator cartItemValidator) 
        : base(httpContextAccessor)
    {
        _cartService = cartService;
        _cartItemValidator = cartItemValidator;
    }

    [HttpGet]
    public IActionResult Items()
    {
        var serviceResponse = _cartService
            .GetItemsAsync(CurrentUserId)
            .Result;
        
        return Ok(serviceResponse.Data);
    }
    
    [HttpGet("{itemId}")]
    public IActionResult Items(int itemId)
    {
        var serviceResponse = _cartService
            .GetItemDetailsAsync(CurrentUserId, itemId)
            .Result;
        
        return serviceResponse.IsSuccessful ? Ok(serviceResponse.Data) : NotFound(itemId);
    }

    [HttpPost]
    public IActionResult Post(CartItem item)
    {
        var validationResult = _cartItemValidator.Validate(item);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var serviceResponse = _cartService
            .AddItemAsync(CurrentUserId, item)
            .Result;
        
        return serviceResponse.IsSuccessful ? Ok(serviceResponse.Data) : BadRequest(serviceResponse.Errors);
    }
    
    [HttpPut("{itemId}")]
    public IActionResult Put(int itemId, CartItem item)
    {
        var validationResult = _cartItemValidator.Validate(item);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var serviceResponse = _cartService
            .UpdateItemAsync(CurrentUserId, itemId, item)
            .Result;
        
        return serviceResponse.IsSuccessful ? Ok(serviceResponse.Data) : BadRequest(serviceResponse.Errors);
    }

    [HttpDelete("{itemId}")]
    public IActionResult Delete(int itemId)
    {
        var serviceResponse = _cartService
            .RemoveItemAsync(CurrentUserId, itemId)
            .Result;
        
        return serviceResponse.IsSuccessful ? Ok(serviceResponse.Data) : BadRequest(serviceResponse.Errors);
    }
    
    [HttpPost("{itemId}/image")]
    public IActionResult Image(int itemId, IFormFile imageFile)
    {
        var imageMetadata = new ItemImage
        {
            FileName = imageFile.FileName,
            ContentType = imageFile.ContentType,
            ContentDisposition = imageFile.ContentDisposition,
            Length = imageFile.Length,
            Name = imageFile.Name
        };
        
        var serviceResponse = _cartService
            .AddImageAsync(CurrentUserId, itemId, imageMetadata)
            .Result;
        
        return serviceResponse.IsSuccessful ? Ok(serviceResponse.Data) : BadRequest(serviceResponse.Errors);
    }
}