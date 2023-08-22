using CartEase.Core.Entity;

namespace CartEase.Models;

public class ItemImageModel : Entity
{
    public string FileName { get; set; }
    public byte[] FileBytes { get; set; }
    public string ContentType { get; set; }
    public string ContentDisposition { get; set; }
    public long Length { get; set; }
    public string Name { get; set; }
}