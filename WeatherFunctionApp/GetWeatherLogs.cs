using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;

namespace WeatherFunctionApp
{
    public static class GetWeatherLogs
    {
        private static readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [FunctionName("GetWeatherLogs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req,
            ILogger log)
        {
            string from = req.Query["from"];
            string to = req.Query["to"];

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) ||
                !DateTime.TryParse(from, out DateTime fromDate) ||
                !DateTime.TryParse(to, out DateTime toDate))
            {
                return new BadRequestObjectResult("Please provide valid 'from' and 'to' query parameters.");
            }

            var tableServiceClient = new TableServiceClient(storageConnectionString);
            var tableClient = tableServiceClient.GetTableClient("WeatherLogs");

            var query = tableClient.QueryAsync<TableEntity>(e =>
                e.Timestamp >= fromDate && e.Timestamp <= toDate);

            //var logs = await query.;// ToListAsync();

            return new OkObjectResult(query);
        }
        //private static readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        //[FunctionName("GetWeatherLogs")]
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
