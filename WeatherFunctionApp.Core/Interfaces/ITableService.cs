using WeatherFunctionApp.Core.Models;

namespace WeatherFunctionApp.Core.Interfaces
{
    public interface ITableService
    {
        Task SaveLogToTableAsync(string partitionKey, string rowKey, DateTimeOffset timeStamp, string status, string message);
        Task<List<WeatherLogEntity>> GetLogsAsync(DateTime from, DateTime to);
    }
}
