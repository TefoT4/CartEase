using AutoMapper;
using CartEase.Core;
using CartEase.Application.Domain;
using CartEase.Application.Validators;
using CartEase.Core.Repository;
using CartEase.Models;
using Microsoft.Extensions.Logging;

namespace CartEase.Application.Service;

public class CartService : ICartService
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly ILogger<CartService> _logger;
    private readonly ItemImageValidator _itemImageValidator;

    public CartService(ILogger<CartService> logger, IMapper mapper, IRepository repository, ItemImageValidator itemImageValidator)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _itemImageValidator = itemImageValidator;
    }
    
    public async Task<ServiceResponse<IEnumerable<CartItem>>> GetItemsAsync(string userId)
    {
        try
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            
            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);
            
            if(user == null)
                return new ServiceResponse<IEnumerable<CartItem>>(){ IsSuccessful = false, Errors = new List<string> { "User not found" } };
            
            var cartItems = _repository.GetAll<CartItemModel>().FirstOrDefault(x => x.UserId == user.Id);
            
            var mappedCartItems = _mapper.Map<IEnumerable<CartItem>>(cartItems);
            
            return new ServiceResponse<IEnumerable<CartItem>>(){ IsSuccessful = true, Data = mappedCartItems };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return await Task.FromResult(new ServiceResponse<IEnumerable<CartItem>>(){ IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<CartItem>> GetItemDetailsAsync(string? userId, int cartItemId)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);
            
            if(user == null)
                return new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { "User not found" } };

            var cartItem = _repository.GetAll<CartItemModel>()
                .FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);
            
            if(cartItem == null)
                return new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { "Cart item not found" } };
            
            return new ServiceResponse<CartItem>(){ IsSuccessful = true, Data = _mapper.Map<CartItem>(cartItem) };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return await Task.FromResult(new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<CartItem>> AddItemAsync(string userId, CartItem cartItemInput)
    {
        try
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);

            if (user == null)
                return new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { "User not found" } };
            
            var cartItem = _mapper.Map<CartItemModel>(cartItemInput);

            cartItem.UserId = user.Id;
            
            var result = await _repository.AddAsync(cartItem, user.Id);
            
            return result != null ? 
                new ServiceResponse<CartItem> { IsSuccessful = true, Data = _mapper.Map<CartItem>(result) } : 
                new ServiceResponse<CartItem> { IsSuccessful = false };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return await Task.FromResult(new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<CartItem>> UpdateItemAsync(string userId, int cartItemId, CartItem cartItemInput)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);
            
            if(user == null)
                return new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { "User not found" } };
            
            var cartItem = _repository.GetAll<CartItemModel>().FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);
            
            if(cartItem == null)
                return new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { "Cart item not found" } };
            
            cartItem.Name = cartItemInput.Name;
            cartItem.Description = cartItemInput.Description;
            cartItem.Price = cartItemInput.Price;
            cartItem.Quantity = cartItemInput.Quantity;
            
            var result = await _repository.UpdateAsync(cartItem, user.Id);
            
            return result != null ? 
                new ServiceResponse<CartItem> { IsSuccessful = true, Data = _mapper.Map<CartItem>(result) } : 
                new ServiceResponse<CartItem> { IsSuccessful = false };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return await Task.FromResult(new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<bool>> RemoveItemAsync(string userId, int cartItemId)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);
            
            if(user == null)
                return new ServiceResponse<bool>(){ IsSuccessful = false, Errors = new List<string> { "User not found" } };
            
            var cartItem = _repository.GetAll<CartItemModel>().FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);
            
            if(cartItem == null)
                return new ServiceResponse<bool>(){ IsSuccessful = false, Errors = new List<string> { "Cart item not found" } };
            
            var result = await _repository.DeleteAsync(cartItem, user.Id);
            
            return result != null ? 
                new ServiceResponse<bool> { IsSuccessful = true } : 
                new ServiceResponse<bool> { IsSuccessful = false };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return await Task.FromResult(new ServiceResponse<bool>(){ IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<CartItem>> AddImageAsync(string userId, int cartItemId, ItemImage itemImageFile)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            var validationResult = _itemImageValidator.Validate(itemImageFile);

            if (!validationResult.IsValid)
                return new ServiceResponse<CartItem>()
                    { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList() };
            
            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);
            
            if(user == null)
                return new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { "User not found" } };

            var cartItem = _repository.GetAll<CartItemModel>().FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);
            
            if(cartItem == null)
                return new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { "Cart item not found" } };
            
            var itemImage = _mapper.Map<ItemImageModel>(itemImageFile);
            
            cartItem.ItemImages.Add(itemImage);
            
            ItemImageModel result = await _repository.UpdateAsync(itemImage, user.Id);

            return result != null ?  
                new ServiceResponse<CartItem>
                {
                    IsSuccessful = true, Data = _mapper.Map<CartItem>(result)
                } : 
                new ServiceResponse<CartItem>
                {
                    IsSuccessful = false, Errors = new List<string> { "Failed to add image" }
                };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return await Task.FromResult(new ServiceResponse<CartItem>(){ IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    private void ValidateAddImageParameters(string userId, int cartItemId)
    {
        if (userId == null)
            throw new ArgumentNullException(nameof(userId));

        if (cartItemId <= 0)
            throw new ArgumentOutOfRangeException(nameof(cartItemId));
    }
}