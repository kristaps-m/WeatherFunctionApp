using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WeatherFunctionApp
{
    public static class GetWeatherPayload
    {
        private static readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [FunctionName("GetWeatherPayload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{logId}")] HttpRequest req,
            string logId,
            ILogger log)
        {
            if (string.IsNullOrEmpty(logId))
            {
                return new BadRequestObjectResult("Please provide a valid logId.");
            }

            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("weatherdata");

            // var blobClient = blobContainerClient.GetBlobClient($"{logId}.json");
            var blobClient = blobContainerClient.GetBlobClient(logId);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();
                using (var reader = new StreamReader(downloadInfo.Value.Content))
                {
                    var content = await reader.ReadToEndAsync();
                    return new OkObjectResult(content);
                }
            }

            return new NotFoundObjectResult("Log not found.");
        }
    }
}
