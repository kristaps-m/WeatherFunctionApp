using WeatherFunctionApp.Core.Models;

namespace WeatherFunctionApp.Core.Interfaces
{
    public interface ITableService
    {
        Task SaveLogToTableAsync(WeatherLogEntity logEntity);
        Task<List<WeatherLogEntity>> GetLogsAsync(DateTime from, DateTime to);
    }
}
