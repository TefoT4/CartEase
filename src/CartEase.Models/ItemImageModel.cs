using CartEase.Core.Entity;

namespace CartEase.Models;

public class ItemImageModel : Entity
{
    public byte[] ImageFile { get; set; }
    public string Description { get; set; }
}