using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherFunctionApp.Core.Interfaces;
using WeatherFunctionApp.Core.Models;

namespace WeatherFunctionApp.Tests
{
    public class GetWeatherLogsTests
    {
        private readonly Mock<ITableService> _mockTableService;
        private readonly Mock<ILogger> _mockLogger;

        public GetWeatherLogsTests()
        {
            _mockTableService = new Mock<ITableService>();
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public async Task GetLogs_ReturnsLogs()
        {
            // Arrange
            var logs = new List<WeatherLogEntity>
            {
                new WeatherLogEntity { PartitionKey = "WeatherLog", RowKey = "1", Status = "Success", Message = "Test message 1" },
                new WeatherLogEntity { PartitionKey = "WeatherLog", RowKey = "2", Status = "Failure", Message = "Test message 2" }
            };
            _mockTableService.Setup(ts => ts.GetLogsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                             .ReturnsAsync(logs);

            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?from=2024-07-29&to=2024-07-31");

            var function = new GetWeatherLogs(_mockTableService.Object);

            // Act
            var result = await function.Run(context.Request, _mockLogger.Object) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnLogs = result.Value as List<WeatherLogEntity>;
            Assert.Equal(2, returnLogs.Count);
        }

        [Fact]
        public async Task GetLogs_InvalidDateFormat_ReturnsBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?from=invalid&to=invalid");

            var function = new GetWeatherLogs(_mockTableService.Object);

            // Act
            var result = await function.Run(context.Request, _mockLogger.Object) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Please provide valid 'from' and 'to' query parameters.", result.Value);
        }
    }
}
