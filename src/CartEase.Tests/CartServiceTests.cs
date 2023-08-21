using CartEase.Core.Repository;
using Microsoft.Extensions.Logging;
using CartEase.Application.Service;
using CartEase.Models;
using CartEase.Application.Domain;
using CartEase.Application.Validators;

namespace CartEase.Tests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<IRepository> _repositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<CartService>> _loggerMock;
        private Mock<ItemImageValidator> _itemImageValidatorMock;

        private CartService _cartService;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CartService>>();
            _itemImageValidatorMock = new Mock<ItemImageValidator>();

            _cartService = new CartService(
                _loggerMock.Object,
                _mapperMock.Object,
                _repositoryMock.Object,
                _itemImageValidatorMock.Object);
        }

        // Tests for GetItemsAsync method

        [Test]
        public async Task GetItemsAsync_WhenUserIdIsNull_ReturnsError()
        {
            var result = await _cartService.GetItemsAsync(null);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Value cannot be null. (Parameter 'userId')"));
        }

        [Test]
        public async Task GetItemsAsync_UserNotFound_ReturnsError()
        {
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns(new List<UserModel>().AsQueryable());

            var result = await _cartService.GetItemsAsync("nonExistentUserId");

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("User not found"));
        }

        [Test]
        public async Task GetItemsAsync_UserFoundButCartItemsNotFound_ReturnsEmptyData()
        {
            var user = new UserModel { AuthProviderId = "1", Id = 1 };

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());

            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel>()).AsQueryable());

            var result = await _cartService.GetItemsAsync("1");

            Assert.That(result.IsSuccessful, Is.EqualTo(true));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Empty);
        }

        [Test]
        public async Task GetItemsAsync_ValidInput_ReturnsMappedCartItems()
        {
            var user = new UserModel { AuthProviderId = "1", Id = 1 };
            var mappedCartItems = new List<CartItem>
            {
                new CartItem()
                {
                    Id = 1,
                    Name = "Test Item 1",
                    Description = "Test Description 1",
                    Price = 2.00m,
                    Quantity = 2
                },
                new CartItem()
                {
                    Id = 2,
                    Name = "Test Item 2",
                    Description = "Test Description 2",
                    Price = 2.00m,
                    Quantity = 2
                }
            };
            
            var cartItems = new List<CartItemModel>
            {
                new CartItemModel()
                {
                    UserId = 1,
                    Id = 1,
                    Name = "Test Item 1",
                    Description = "Test Description 1",
                    Price = 2.00m,
                    Quantity = 2
                },
                new CartItemModel()
                {
                    UserId = 1,
                    Id = 2,
                    Name = "Test Item 2",
                    Description = "Test Description 2",
                    Price = 2.00m,
                    Quantity = 2
                },
                new CartItemModel()
                {
                    UserId = 2,
                    Id = 3,
                    Name = "Test Item 3",
                    Description = "Test Description 3",
                    Price = 2.00m,
                    Quantity = 2
                }
            };

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());

            // Setup the repository to return only cart items associated with the user
            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((cartItems.Where(item => item.UserId == user.Id)).AsQueryable());

            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<CartItem>>(It.IsAny<IEnumerable<CartItemModel>>()))
                .Returns(mappedCartItems);

            var result = await _cartService.GetItemsAsync("1");

            Assert.That(result.IsSuccessful, Is.EqualTo(true));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count(), Is.EqualTo(2));
        }

        // Tests for GetItemDetailsAsync method
        [Test]
        public async Task GetItemDetailsAsync_UserIdIsNull_ReturnsError()
        {
            var result = await _cartService.GetItemDetailsAsync(null, 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Value cannot be null. (Parameter 'userId')"));
        }

        [Test]
        public async Task GetItemDetailsAsync_CartItemIdIsZero_ReturnsError()
        {
            var result = await _cartService.GetItemDetailsAsync("userId", 0);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Specified argument was out of the range of valid values. (Parameter 'cartItemId')"));
        }

        [Test]
        public async Task GetItemDetailsAsync_UserNotFound_ReturnsError()
        {
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns(new List<UserModel>().AsQueryable());

            var result = await _cartService.GetItemDetailsAsync("nonExistentUserId", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("User not found"));
        }

        [Test]
        public async Task GetItemDetailsAsync_CartItemNotFound_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());

            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel>()).AsQueryable());

            var result = await _cartService.GetItemDetailsAsync("userId", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Cart item not found"));
        }

        // Tests for GetItemDetailsAsync method
        [Test]
        public async Task GetItemDetailsAsync_ValidInput_ReturnsMappedCartItem()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItem = new CartItemModel { Id = 1, UserId = 1 };
            var mappedCartItem = new CartItem { Id = 1 };

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());

            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel> { cartItem }).AsQueryable());

            _mapperMock.Setup(mapper => mapper.Map<CartItem>(cartItem)).Returns(mappedCartItem);

            var result = await _cartService.GetItemDetailsAsync("userId", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(true));
            Assert.That(result.Data, Is.EqualTo(mappedCartItem));
        }

        [Test]
        public async Task GetItemDetailsAsync_UserFoundButCartItemIdDoesNotMatch_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItem = new CartItemModel { Id = 1, UserId = 2 };

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());

            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel> { cartItem }).AsQueryable());

            var result = await _cartService.GetItemDetailsAsync("userId", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Cart item not found"));
        }

        [Test]
        public async Task GetItemDetailsAsync_ExceptionOccurs_ReturnsError()
        {
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>()).Throws<Exception>();

            var result = await _cartService.GetItemDetailsAsync("userId", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First().Contains("Exception"));
        }

        // Tests for AddItemAsync method
        [Test]
        public async Task AddItemAsync_UserIdIsNull_ReturnsError()
        {
            var result = await _cartService.AddItemAsync(null, new CartItem());

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Value cannot be null. (Parameter 'userId')"));
        }

        [Test]
        public async Task AddItemAsync_UserNotFound_ReturnsError()
        {
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns(new List<UserModel>().AsQueryable());

            var result = await _cartService.AddItemAsync("nonExistentUserId", new CartItem());

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("User not found"));
        }

        [Test]
        public async Task AddItemAsync_RepositoryReturnsNull_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItemInput = new CartItem();

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _mapperMock.Setup(mapper => mapper.Map<CartItemModel>(cartItemInput))
                .Returns(new CartItemModel());
            
            _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CartItemModel>(), new CancellationToken()))
                .ReturnsAsync((CartItemModel)null);

            var result = await _cartService.AddItemAsync("userId", cartItemInput);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
        }

        // Tests for AddItemAsync method
        [Test]
        public async Task AddItemAsync_UserFoundButRepositoryReturnsNull_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItemInput = new CartItem();
            var cartItemModel = new CartItemModel();

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _mapperMock.Setup(mapper => mapper.Map<CartItemModel>(cartItemInput)).Returns(cartItemModel);
            
            _repositoryMock.Setup(repo => repo.AddAsync(cartItemModel, new CancellationToken()))
                .ReturnsAsync((CartItemModel)null);

            var result = await _cartService.AddItemAsync("userId", cartItemInput);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task AddItemAsync_MapperReturnsNull_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItemInput = new CartItem();

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _mapperMock.Setup(mapper => mapper.Map<CartItemModel>(cartItemInput)).Returns((CartItemModel)null);

            var result = await _cartService.AddItemAsync("userId", cartItemInput);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task AddItemAsync_ExceptionOccurs_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItemInput = new CartItem();

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _mapperMock.Setup(mapper => mapper.Map<CartItemModel>(cartItemInput)).Returns(new CartItemModel());
            
            _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CartItemModel>(), new CancellationToken()))
                .Throws<Exception>();

            var result = await _cartService.AddItemAsync("userId", cartItemInput);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Data, Is.Null);
        }

        // Tests for UpdateItemAsync method
        [Test]
        public async Task UpdateItemAsync_UserIdIsNull_ReturnsError()
        {
            var result = await _cartService.UpdateItemAsync(null, 1, new CartItem());

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Value cannot be null. (Parameter 'userId')"));
        }

        [Test]
        public async Task UpdateItemAsync_CartItemIdIsZero_ReturnsError()
        {
            var result = await _cartService.UpdateItemAsync("userId", 0, new CartItem());

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Specified argument was out of the range of valid values. (Parameter 'cartItemId')"));
        }

        [Test]
        public async Task UpdateItemAsync_UserNotFound_ReturnsError()
        {
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns(new List<UserModel>().AsQueryable());

            var result = await _cartService.UpdateItemAsync("nonExistentUserId", 1, new CartItem());

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("User not found"));
        }

        [Test]
        public async Task UpdateItemAsync_CartItemNotFound_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns(new List<CartItemModel>().AsQueryable());

            var result = await _cartService.UpdateItemAsync("userId", 1, new CartItem());

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Cart item not found"));
        }

        [Test]
        public async Task UpdateItemAsync_ValidInput_ReturnsSuccessResponse()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItemInput = new CartItem();
            var cartItemModel = new CartItemModel { UserId = 1, Id = 1, Name = "name", Price = 1, Quantity = 1};

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel> { cartItemModel }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.UpdateAsync(cartItemModel, new CancellationToken()))
                .ReturnsAsync(cartItemModel);
            
            _mapperMock.Setup(mapper => mapper.Map<CartItem>(cartItemModel)).Returns(new CartItem());

            var result = await _cartService.UpdateItemAsync("userId", 1, cartItemInput);

            Assert.That(result.IsSuccessful, Is.EqualTo(true));
            Assert.That(result.Data, Is.Not.Null);
        }

        [Test]
        public async Task UpdateItemAsync_RepositoryReturnsNull_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItemInput = new CartItem();
            var cartItemModel = new CartItemModel { UserId = 1 };

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel> { cartItemModel }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<CartItemModel>(), new CancellationToken()))
                .ReturnsAsync((CartItemModel)null);

            var result = await _cartService.UpdateItemAsync("userId", 1, cartItemInput);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
        }

        // Tests for RemoveItemAsync method
        [Test]
        public async Task RemoveItemAsync_UserIdIsNull_ReturnsError()
        {
            var result = await _cartService.RemoveItemAsync(null, 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Value cannot be null. (Parameter 'userId')"));
        }

        [Test]
        public async Task RemoveItemAsync_CartItemIdIsZero_ReturnsError()
        {
            var result = await _cartService.RemoveItemAsync("userId", 0);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Specified argument was out of the range of valid values. (Parameter 'cartItemId')"));
        }

        [Test]
        public async Task RemoveItemAsync_UserNotFound_ReturnsError()
        {
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns(new List<UserModel>().AsQueryable());

            var result = await _cartService.RemoveItemAsync("nonExistentUserId", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("User not found"));
        }

        [Test]
        public async Task RemoveItemAsync_CartItemNotFound_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns(new List<CartItemModel>().AsQueryable());

            var result = await _cartService.RemoveItemAsync("userId", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(false));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors.First(), Is.EqualTo("Cart item not found"));
        }

        [Test]
        public async Task RemoveItemAsync_ValidInput_ReturnsSuccessResponse()
        {
            var user = new UserModel { AuthProviderId = "1", Id = 1 };
            var cartItemModel = new CartItemModel { UserId = 1, Id = 1, Name = "item1", Description = "item1", Price = 1, Quantity = 1};
            
            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel> { cartItemModel }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.DeleteAsync(cartItemModel, new CancellationToken()))
                .ReturnsAsync(true);

            var result = await _cartService.RemoveItemAsync("1", 1);

            Assert.That(result.IsSuccessful, Is.EqualTo(true));
        }

        [Test]
        public async Task RemoveItemAsync_RepositoryReturnsFalse_ReturnsError()
        {
            var user = new UserModel { AuthProviderId = "userId", Id = 1 };
            var cartItemModel = new CartItemModel { UserId = 1 };

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { user }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.GetAll<CartItemModel>())
                .Returns((new List<CartItemModel> { cartItemModel }).AsQueryable());
            
            _repositoryMock.Setup(repo => repo.DeleteAsync(cartItemModel, new CancellationToken()))
                .ReturnsAsync(false);

            var result = await _cartService.RemoveItemAsync("userId", 1);

            Assert.IsFalse(result.IsSuccessful);
        }

        // Tests for AddImageAsync method
        [Test]
        public async Task AddItemAsync_ValidInput_ReturnsSuccessResponse()
        {
            var cartItemInput = new CartItem();
            var cartItemModel = new CartItemModel();

            _repositoryMock.Setup(repo => repo.GetAll<UserModel>())
                .Returns((new List<UserModel> { new UserModel { AuthProviderId = "userId", Id = 1 } }).AsQueryable());

            _mapperMock.Setup(mapper => mapper.Map<CartItemModel>(cartItemInput))
                .Returns(cartItemModel);

            _repositoryMock.Setup(repo => repo.AddAsync(cartItemModel, new CancellationToken()))
                .ReturnsAsync(cartItemModel);

            _mapperMock.Setup(mapper => mapper.Map<CartItem>(cartItemModel)).Returns(new CartItem());

            var result = await _cartService.AddItemAsync("userId", cartItemInput);

            Assert.That(result.IsSuccessful, Is.EqualTo(true));
            Assert.That(result.Data, Is.Not.Null);
        }

        // Tests for Utility methods
        [Test]
        public void ValidateAddImageParameters_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _cartService.ValidateAddImageParameters(null, 1));
        }

        [Test]
        public void ValidateAddImageParameters_WhenCartItemIdIsZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _cartService.ValidateAddImageParameters("userId", 0));
        }
    }
}
