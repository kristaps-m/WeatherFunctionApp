using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeatherFunctionApp.Core.Models;
using WeatherFunctionApp.Infrastructure.Services;

namespace WeatherFunctionApp
{
    public class FetchWeatherData
    {
        private readonly string openWeatherMapApiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");
        private readonly WeatherService _weatherService;
        private readonly BlobService _blobService;
        private readonly TableService _tableService;

        public FetchWeatherData(WeatherService weatherService, BlobService blobService, TableService tableService)
        {
            _weatherService = weatherService;
            _blobService = blobService;
            _tableService = tableService;
        }

        [FunctionName("FetchWeatherData")]
        public async Task Run([TimerTrigger("*/1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q=London&appid={openWeatherMapApiKey}";
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
