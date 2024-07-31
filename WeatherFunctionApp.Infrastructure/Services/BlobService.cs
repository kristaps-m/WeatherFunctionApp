using Azure.Storage.Blobs;
using WeatherFunctionApp.Core.Interfaces;

namespace WeatherFunctionApp.Infrastructure.Services
{
    public class BlobService : IBlobService
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
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(new BinaryData(content));
        }

        public async Task<string> GetPayloadFromBlobAsync(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();

                return await new StreamReader(downloadInfo.Value.Content).ReadToEndAsync();
            }

            return null;
        }
    }
}
