namespace CartEase.Api.Domain;

public class ImageMetadata
{
    public string FileName { get; set; }
    public byte[] FileBytes { get; set; }
    public string ContentType { get; set; }
}