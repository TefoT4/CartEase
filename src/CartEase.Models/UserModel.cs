
using CartEase.Core.Entity;

namespace CartEase.Models
{
    public class UserModel : Entity
    {
        public string Username { get; init; }
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string? AuthProviderId { get; set; }

        public ICollection<CartItemModel> CartItems { get; init; }
    }
}