using CartEase.Application.Domain;
using CartEase.Core;

namespace CartEase.Application.Service;

public interface ICartService
{
    Task<ServiceResponse<IEnumerable<CartItem>>> GetItemsAsync(string userId);
    
    Task<ServiceResponse<CartItem>> GetItemDetailsAsync(string userId, int id);
    
    Task<ServiceResponse<CartItem>> AddItemAsync(string userId, CartItem cartItemInput);
    
    Task<ServiceResponse<CartItem>> UpdateItemAsync(string userId, int cartItemId, CartItem cartItemInput);
    
    Task<ServiceResponse<bool>> RemoveItemAsync(string userId, int cartItemId);
    
    Task<ServiceResponse<CartItem>> AddImageAsync(string userId, int cartItemId, ItemImage itemImageFile);
}