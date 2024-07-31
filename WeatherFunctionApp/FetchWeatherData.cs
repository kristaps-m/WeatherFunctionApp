using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherFunctionApp.Core.Models;
using WeatherFunctionApp.Infrastructure.Services;

namespace WeatherFunctionApp
{
    public class FetchWeatherData
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
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
            //log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            string url = $"https://api.openweathermap.org/data/2.5/weather?q=London&appid={openWeatherMapApiKey}";
            string logId = Guid.NewGuid().ToString();

            try
            {
                // var response = await httpClient.GetAsync(url); // This inside service;
                // var content = await response.Content.ReadAsStringAsync();
                var content = await _weatherService.FetchWeatherDataAsync(url);
                await _blobService.SavePayloadToBlobAsync(content, logId);
                await _tableService.SaveLogToTableAsync(new WeatherLogEntity
                {
                    PartitionKey = "WeatherLog",
                    RowKey = logId,
                    Status = "Success",
                    Message = content
                });
                log.LogInformation($"Successfully fetched weather data at: {DateTime.Now}");

                //if (response.IsSuccessStatusCode)
                //{
                //    await SavePayloadToBlobAsync(content, logId);
                //    await SaveLogToTableAsync("Success", content, logId);
                //    log.LogInformation($"Successfully fetched weather data at: {DateTime.Now}");
                //}
                //else
                //{
                //    await SaveLogToTableAsync("Failure", content, logId);
                //    log.LogError($"Failed to fetch weather data at: {DateTime.Now}");
                //}
            }
            catch (Exception ex)
            {
                await _tableService.SaveLogToTableAsync(new WeatherLogEntity
                {
                    PartitionKey = "WeatherLog",
                    RowKey = logId,
                    Status = "Failure",
                    Message = ex.Message
                });

                log.LogError($"Exception occurred: {ex.Message}");
                //await SaveLogToTableAsync("Failure", ex.Message, logId);
                //log.LogError($"Exception occurred: {ex.Message}");
            }

            //async Task SavePayloadToBlobAsync(string content, string blobName)
            //{
            //    var blobServiceClient = new BlobServiceClient(storageConnectionString);
            //    var blobContainerClient = blobServiceClient.GetBlobContainerClient("weatherdata");
            //    await blobContainerClient.CreateIfNotExistsAsync();
            //    // var blobClient = blobContainerClient.GetBlobClient($"{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            //    var blobClient = blobContainerClient.GetBlobClient(blobName);
            //    await blobClient.UploadAsync(new BinaryData(content));
            //}

            //async Task SaveLogToTableAsync(string status, string message, string blobName)
            //{
            //    var tableServiceClient = new TableServiceClient(storageConnectionString);
            //    var tableClient = tableServiceClient.GetTableClient("WeatherLogs");
            //    await tableClient.CreateIfNotExistsAsync();

            //    var logEntity = new WeatherLogEntity
            //    {
            //        PartitionKey = "WeatherLog",
            //        RowKey = blobName,
            //        Timestamp = DateTime.UtcNow,
            //        Status = status,
            //        Message = message
            //    };

            //    await tableClient.AddEntityAsync(logEntity);
            //}
        }
    }
}
