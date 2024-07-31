namespace WeatherFunctionApp.Core.Interfaces
{
    public interface IBlobService
    {
        Task SavePayloadToBlobAsync(string content, string blobName);
        Task<string> GetPayloadFromBlobAsync(string blobName);
    }
}
