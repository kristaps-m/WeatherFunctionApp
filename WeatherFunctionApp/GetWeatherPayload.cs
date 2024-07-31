using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WeatherFunctionApp.Core.Interfaces;

namespace WeatherFunctionApp
{
    public class GetWeatherPayload
    {
        private readonly IBlobService _blobService;

        public GetWeatherPayload(IBlobService blobService)
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
;           
            if (content != null)
            {
                return new OkObjectResult(content);
            }

            return new NotFoundObjectResult("Log not found.");
        }
    }
}
