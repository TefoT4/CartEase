using AutoMapper;
using CartEase.Application.Domain;
using CartEase.Application.Validators;
using CartEase.Core;
using CartEase.Core.Repository;
using CartEase.Models;
using Microsoft.Extensions.Logging;

namespace CartEase.Application.Services.Cart;

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
            {
                throw new ArgumentNullException(nameof(userId));
            }

            _logger.LogInformation("Getting items for user {UserId}", userId);

            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return new ServiceResponse<IEnumerable<CartItem>>()
                {
                    IsSuccessful = false,
                    Errors = new List<string> { "User not found" }
                };
            }

            var cartItems = _repository.GetAll<CartItemModel>().Where(x => x.UserId == user.Id);

            var mappedCartItems = _mapper.Map<IEnumerable<CartItem>>(cartItems);

            var enumerable = mappedCartItems.ToList();
            _logger.LogInformation("Retrieved {ItemCount} items for user {UserId}", enumerable.Count(), userId);

            return new ServiceResponse<IEnumerable<CartItem>>()
            {
                IsSuccessful = true,
                Data = enumerable
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while getting items for user {UserId}", userId);
            return await Task.FromResult(new ServiceResponse<IEnumerable<CartItem>>()
            {
                IsSuccessful = false,
                Errors = new List<string> { e.Message }
            });
        }
    }

    public async Task<ServiceResponse<CartItem>> GetItemDetailsAsync(string userId, int cartItemId)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            _logger.LogInformation("Getting item details for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);

            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return new ServiceResponse<CartItem>()
                {
                    IsSuccessful = false,
                    Errors = new List<string> { "User not found" }
                };
            }

            var cartItem = _repository.GetAll<CartItemModel>()
                .FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);

            if (cartItem == null)
            {
                _logger.LogWarning("Cart item with ID {CartItemId} not found for user {UserId}", cartItemId, userId);
                return new ServiceResponse<CartItem>()
                {
                    IsSuccessful = false,
                    Errors = new List<string> { "Cart item not found" }
                };
            }

            _logger.LogInformation("Retrieved details for cart item ID {CartItemId} for user {UserId}", cartItemId, userId);

            return new ServiceResponse<CartItem>()
            {
                IsSuccessful = true,
                Data = _mapper.Map<CartItem>(cartItem)
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while getting item details for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
            return await Task.FromResult(new ServiceResponse<CartItem>()
            {
                IsSuccessful = false,
                Errors = new List<string> { e.Message }
            });
        }
    }

    public async Task<ServiceResponse<CartItem>> AddItemAsync(string userId, CartItem cartItemInput)
    {
        try
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            _logger.LogInformation("Adding item for user {UserId}", userId);

            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { "User not found" } };
            }

            var cartItem = _mapper.Map<CartItemModel>(cartItemInput);

            cartItem.UserId = user.Id;

            var result = await _repository.AddAsync(cartItem);

            if (result != null)
            {
                _logger.LogInformation("Item added successfully for user {UserId}, cart item ID {CartItemId}", userId, result.Id);
                return new ServiceResponse<CartItem> { IsSuccessful = true, Data = _mapper.Map<CartItem>(result) };
            }
            else
            {
                _logger.LogError("Failed to add item for user {UserId}", userId);
                return new ServiceResponse<CartItem> { IsSuccessful = false };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while adding item for user {UserId}", userId);
            return await Task.FromResult(new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<CartItem>> UpdateItemAsync(string userId, int cartItemId, CartItem cartItemInput)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            _logger.LogInformation("Updating item for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);

            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { "User not found" } };
            }

            var cartItem = _repository.GetAll<CartItemModel>().FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);

            if (cartItem == null)
            {
                _logger.LogWarning("Cart item with ID {CartItemId} not found for user {UserId}", cartItemId, userId);
                return new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { "Cart item not found" } };
            }

            cartItem.Name = cartItemInput.Name;
            cartItem.Description = cartItemInput.Description;
            cartItem.Price = cartItemInput.Price;
            cartItem.Quantity = cartItemInput.Quantity;

            var result = await _repository.UpdateAsync(cartItem);

            if (result != null)
            {
                _logger.LogInformation("Item updated successfully for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
                return new ServiceResponse<CartItem> { IsSuccessful = true, Data = _mapper.Map<CartItem>(result) };
            }
            else
            {
                _logger.LogError("Failed to update item for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
                return new ServiceResponse<CartItem> { IsSuccessful = false };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while updating item for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
            return await Task.FromResult(new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<bool>> RemoveItemAsync(string userId, int cartItemId)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            _logger.LogInformation("Removing item for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);

            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return new ServiceResponse<bool>() { IsSuccessful = false, Errors = new List<string> { "User not found" } };
            }

            var cartItem = _repository.GetAll<CartItemModel>().FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);

            if (cartItem == null)
            {
                _logger.LogWarning("Cart item with ID {CartItemId} not found for user {UserId}", cartItemId, userId);
                return new ServiceResponse<bool>() { IsSuccessful = false, Errors = new List<string> { "Cart item not found" } };
            }

            var result = await _repository.DeleteAsync(cartItem);

            _logger.LogInformation("Item removed successfully for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);

            return new ServiceResponse<bool> { IsSuccessful = result };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while removing item for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
            return await Task.FromResult(new ServiceResponse<bool>() { IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public async Task<ServiceResponse<CartItem>> AddImageAsync(string userId, int cartItemId, ItemImage itemImageFile)
    {
        try
        {
            ValidateAddImageParameters(userId, cartItemId);
            
            _logger.LogInformation("Adding image for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);

            var validationResult = await _itemImageValidator.ValidateAsync(itemImageFile);

            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("Validation errors encountered while adding image for user {UserId}, cart item ID {CartItemId}: {ValidationErrors}", userId, cartItemId, string.Join(", ", validationErrors));
                return new ServiceResponse<CartItem>()
                {
                    IsSuccessful = false, Errors = validationErrors
                };
            }

            var user = _repository.GetAll<UserModel>().FirstOrDefault(x => x.AuthProviderId == userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { "User not found" } };
            }

            var cartItem = _repository.GetAll<CartItemModel>().FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);

            if (cartItem == null)
            {
                _logger.LogWarning("Cart item with ID {CartItemId} not found for user {UserId}", cartItemId, userId);
                return new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { "Cart item not found" } };
            }

            var itemImage = _mapper.Map<ItemImageModel>(itemImageFile);

            cartItem.ItemImages.Add(itemImage);

            ItemImageModel result = await _repository.UpdateAsync(itemImage);

            if (result != null)
            {
                _logger.LogInformation("Image added successfully for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
                return new ServiceResponse<CartItem>
                {
                    IsSuccessful = true, Data = _mapper.Map<CartItem>(result)
                };
            }
            else
            {
                _logger.LogError("Failed to add image for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
                return new ServiceResponse<CartItem>
                {
                    IsSuccessful = false, Errors = new List<string> { "Failed to add image" }
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while adding image for user {UserId}, cart item ID {CartItemId}", userId, cartItemId);
            return await Task.FromResult(new ServiceResponse<CartItem>() { IsSuccessful = false, Errors = new List<string> { e.Message } });
        }
    }

    public void ValidateAddImageParameters(string userId, int cartItemId)
    {
        _logger.LogDebug("Validating parameters for adding image: user ID {UserId}, cart item ID {CartItemId}", userId, cartItemId);

        if (userId == null)
        {
            _logger.LogError("User ID is null");
            throw new ArgumentNullException(nameof(userId));
        }

        if (cartItemId <= 0)
        {
            _logger.LogError("Cart item ID is not valid: {CartItemId}", cartItemId);
            throw new ArgumentOutOfRangeException(nameof(cartItemId));
        }
    }
}