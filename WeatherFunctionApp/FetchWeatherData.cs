using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherFunctionApp.Models;

namespace WeatherFunctionApp
{
    public class FetchWeatherData
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly string openWeatherMapApiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");

        [FunctionName("FetchWeatherData")]
        public async Task Run([TimerTrigger("*/1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            //log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            string url = $"https://api.openweathermap.org/data/2.5/weather?q=London&appid={openWeatherMapApiKey}";

            try
            {
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await SavePayloadToBlobAsync(content);
                    await SaveLogToTableAsync("Success", content);
                    log.LogInformation($"Successfully fetched weather data at: {DateTime.Now}");
                }
                else
                {
                    await SaveLogToTableAsync("Failure", content);
                    log.LogError($"Failed to fetch weather data at: {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                await SaveLogToTableAsync("Failure", ex.Message);
                log.LogError($"Exception occurred: {ex.Message}");
            }

            async Task SavePayloadToBlobAsync(string content)
            {
                var blobServiceClient = new BlobServiceClient(storageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("weatherdata");
                await blobContainerClient.CreateIfNotExistsAsync();
                var blobClient = blobContainerClient.GetBlobClient($"{DateTime.UtcNow:yyyyMMddHHmmss}.json");
                await blobClient.UploadAsync(new BinaryData(content));
            }

            async Task SaveLogToTableAsync(string status, string message)
            {
                var tableServiceClient = new TableServiceClient(storageConnectionString);
                var tableClient = tableServiceClient.GetTableClient("WeatherLogs");
                await tableClient.CreateIfNotExistsAsync();

                //var logEntity = new Azure.Data.Tables.TableEntity
                //{
                //    PartitionKey = "WeatherLog",
                //    RowKey = Guid.NewGuid().ToString(),
                //    Timestamp = DateTime.UtcNow,
                //    //{ "Status", status },
                //    //{ "Message", message }
                //};
                var logEntity = new WeatherLogEntity
                {
                    PartitionKey = "WeatherLog",
                    RowKey = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    Status = status,
                    Message = message
                };

                await tableClient.AddEntityAsync(logEntity);
            }
        }
    }
}
