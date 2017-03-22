namespace AInBox.Astove.Core.Model
{
    public interface IResultModel
    {
        bool IsValid { get; set; }
        string Message { get; set; }
        int StatusCode { get; set; }
    }
}
