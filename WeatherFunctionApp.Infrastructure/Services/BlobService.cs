using Azure.Storage.Blobs;

namespace WeatherFunctionApp.Infrastructure.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _blobContainerClient;
        public BlobService(string storageConnectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _blobContainerClient.CreateIfNotExists();
        }

        public async Task SavePayloadToBlobAsync(string content, string blobName)
        {
            //var blobServiceClient = new BlobServiceClient(storageConnectionString);
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            //await blobContainerClient.CreateIfNotExistsAsync();
            // var blobClient = blobContainerClient.GetBlobClient($"{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            //var blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(new BinaryData(content));
        }

        public async Task<string> GetPayloadFromBlobAsync(string logId) //, string? blobName
        {
            var blobClient = _blobContainerClient.GetBlobClient(logId);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();
                return await new StreamReader(downloadInfo.Value.Content).ReadToEndAsync();
                //using (var reader = new StreamReader(downloadInfo.Value.Content))
                //{
                //    var content = await reader.ReadToEndAsync();
                //    return new OkObjectResult(content);
                //}
            }

            return null;
        }
    }
}
