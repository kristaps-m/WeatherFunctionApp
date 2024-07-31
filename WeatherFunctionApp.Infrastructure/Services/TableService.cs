using Azure.Data.Tables;
using WeatherFunctionApp.Core.Interfaces;
using WeatherFunctionApp.Core.Models;

namespace WeatherFunctionApp.Infrastructure.Services
{
    public class TableService : ITableService
    {
        private readonly TableClient _tableClient;

        public TableService(string connectionString, string tableName)
        {
            var tableServiceClient = new TableServiceClient(connectionString);
            _tableClient = tableServiceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task SaveLogToTableAsync(WeatherLogEntity logEntity)
        {
            await _tableClient.AddEntityAsync(logEntity);
        }

        public async Task<List<WeatherLogEntity>> GetLogsAsync(DateTime from, DateTime to)
        {
            var query = _tableClient.QueryAsync<WeatherLogEntity>(e =>
                e.Timestamp >= from && e.Timestamp <= to);
            var logs = new List<WeatherLogEntity>();
            await foreach (var log in query)
            {
                logs.Add(log);
            }
            return logs;
        }
    }
}
