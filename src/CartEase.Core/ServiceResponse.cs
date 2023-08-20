namespace CartEase.Core;

public class ServiceResponse<T>
{
    public ServiceResponse()
    {
        IsSuccessful = true;
        Errors = new List<string>();
    }

    public T Data { get; set; }

    public dynamic Tag { get; set; }
        
    public bool IsSuccessful { get; set; }

    public List<string> Errors { get; set; }
}