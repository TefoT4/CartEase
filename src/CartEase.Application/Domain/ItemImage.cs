namespace CartEase.Application.Domain;

public class ItemImage
{
    public string FileName { get; set; }
    public byte[] FileBytes { get; set; }
    public string ContentType { get; set; }
    public string ContentDisposition { get; set; }
    public long Length { get; set; }
    public string Name { get; set; }
}