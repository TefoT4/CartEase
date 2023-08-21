using System.Collections.ObjectModel;
using CartEase.Core.Entity;

namespace CartEase.Models
{
    public class CartItemModel : Entity
    {
        public CartItemModel()
        {
            ItemImages = new Collection<ItemImageModel>();
        }
        
        public string Name { get; set; }

        public string Description { get; set; }
        
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        public virtual ICollection<ItemImageModel> ItemImages { get; set; }
        
        public int UserId { get; set; }
        
        public virtual UserModel User { get; set; }
    }
}