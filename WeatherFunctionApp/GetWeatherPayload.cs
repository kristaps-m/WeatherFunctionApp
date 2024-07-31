using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using WeatherFunctionApp.Infrastructure.Services;

namespace WeatherFunctionApp
{
    public class GetWeatherPayload
    {
        private static readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly BlobService _blobService;

        public GetWeatherPayload(BlobService blobService)
        {
            _blobService = blobService;
        }

        [FunctionName("GetWeatherPayload")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{logId}")] HttpRequest req,
            string logId,
            ILogger log)
        {
            if (string.IsNullOrEmpty(logId))
            {
                return new BadRequestObjectResult("Please provide a valid logId.");
            }

            var content = await _blobService.GetPayloadFromBlobAsync(logId);
            if (content != null)
            {
                return new OkObjectResult(content);
            }

            return new NotFoundObjectResult("Log not found.");

            //var blobServiceClient = new BlobServiceClient(storageConnectionString);
            //var blobContainerClient = blobServiceClient.GetBlobContainerClient("weatherdata");

            // var blobClient = blobContainerClient.GetBlobClient($"{logId}.json");
            //var blobClient = blobContainerClient.GetBlobClient(logId);

            //if (await blobClient.ExistsAsync())
            //{
            //    var downloadInfo = await blobClient.DownloadAsync();
            //    using (var reader = new StreamReader(downloadInfo.Value.Content))
            //    {
            //        var content = await reader.ReadToEndAsync();
            //        return new OkObjectResult(content);
            //    }
            //}

            //return new NotFoundObjectResult("Log not found.");
        }
    }
}
