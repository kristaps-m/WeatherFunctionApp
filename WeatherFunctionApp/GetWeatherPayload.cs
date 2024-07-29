using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;

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

            var blobClient = blobContainerClient.GetBlobClient($"{logId}.json");

            if (await blobClient.ExistsAsync())
            {
                var content = await blobClient.DownloadContentAsync();
                return new OkObjectResult(content.Value.Content.ToString());
            }
            else
            {
                return new NotFoundObjectResult("Log not found.");
            }
        }


        //[FunctionName("GetWeatherPayload")]
        //public static async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    string responseMessage = string.IsNullOrEmpty(name)
        //        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //        : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //    return new OkObjectResult(responseMessage);
        //}
    }
}
