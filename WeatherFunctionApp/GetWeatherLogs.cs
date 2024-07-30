using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeatherFunctionApp.Models;

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

            var query = tableClient.QueryAsync<WeatherLogEntity>(e =>
                e.Timestamp >= fromDate && e.Timestamp <= toDate);

            //var logs = await query.;// ToListAsync();

            return new OkObjectResult(query);
        }
    }
}