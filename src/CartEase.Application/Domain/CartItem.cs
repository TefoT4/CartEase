using CartEase.Core.Entity;

namespace CartEase.Application.Domain
{
    public class CartItem : Entity
    {
        public string Name { get; init; }

        public string Description { get; set; }
        
        public decimal Price { get; init; }
        
        public int Quantity { get; init; }
    }
}