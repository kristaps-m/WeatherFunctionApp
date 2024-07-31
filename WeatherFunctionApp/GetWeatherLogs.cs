using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeatherFunctionApp.Core.Interfaces;

namespace WeatherFunctionApp
{
    public class GetWeatherLogs
    {
        private readonly ITableService _tableService;

        public GetWeatherLogs(ITableService tableService)
        {
            _tableService = tableService;
        }

        [FunctionName("GetWeatherLogs")]
        public async Task<IActionResult> Run(
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

            var logs = await _tableService.GetLogsAsync(fromDate, toDate);

            return new OkObjectResult(logs);
        }
    }
}
