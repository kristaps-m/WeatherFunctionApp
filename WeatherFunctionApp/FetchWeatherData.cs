using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeatherFunctionApp.Core.Interfaces;
using WeatherFunctionApp.Core.Models;

namespace WeatherFunctionApp
{
    public class FetchWeatherData
    {
        private readonly IWeatherService _weatherService;
        private readonly IBlobService _blobService;
        private readonly ITableService _tableService;
        private readonly string _openWeatherMapApiKey;

        public FetchWeatherData(IWeatherService weatherService, IBlobService blobService, ITableService tableService)
        {
            _weatherService = weatherService;
            _blobService = blobService;
            _tableService = tableService;
            _openWeatherMapApiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");
        }

        [FunctionName("FetchWeatherData")]
        public async Task Run([TimerTrigger("*/1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            string url = $"weather?q=London&appid={_openWeatherMapApiKey}";
            string logId = Guid.NewGuid().ToString();

            try
            {
                var content = await _weatherService.FetchWeatherDataAsync(url);
                await _blobService.SavePayloadToBlobAsync(content, logId);
                await _tableService.SaveLogToTableAsync(new WeatherLogEntity
                {
                    PartitionKey = "WeatherLog",
                    RowKey = logId,
                    Timestamp = DateTime.UtcNow,
                    Status = "Success",
                    Message = content
                });
                log.LogInformation($"Successfully fetched weather data at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                await _tableService.SaveLogToTableAsync(new WeatherLogEntity
                {
                    PartitionKey = "WeatherLog",
                    RowKey = logId,
                    Timestamp = DateTime.UtcNow,
                    Status = "Failure",
                    Message = ex.Message
                });

                log.LogError($"Exception occurred: {ex.Message}");
            }
        }
    }
}
