using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherFunctionApp;
using WeatherFunctionApp.Core.Interfaces;
using WeatherFunctionApp.Infrastructure.Services;
using Xunit;

namespace WeatherFunctionApp.Tests
{
    public class FetchWeatherDataTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<ITableService> _mockTableService;
        private readonly Mock<ILogger> _mockLogger;

        public FetchWeatherDataTests()
        {
            //_mockWeatherService = new Mock<IWeatherService>(new HttpClient());
            //_mockBlobService = new Mock<IBlobService>("UseDevelopmentStorage=true", "weatherdata");
            //_mockTableService = new Mock<ITableService>("UseDevelopmentStorage=true", "WeatherLogs");
            //_mockLogger = new Mock<ILogger>();
            _mockWeatherService = new Mock<IWeatherService>();
            _mockBlobService = new Mock<IBlobService>();
            _mockTableService = new Mock<ITableService>();
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public async Task FetchWeatherData_Success()
        {
            // Arrange
            string testData = "{\"weather\":\"clear\"}";
            _mockWeatherService.Setup(ws => ws.FetchWeatherDataAsync(It.IsAny<string>()))
                               .ReturnsAsync(testData);
            var function = new FetchWeatherData(_mockWeatherService.Object, _mockBlobService.Object, _mockTableService.Object);

            // Act
            await function.Run(null, _mockLogger.Object);

            // Assert
            _mockWeatherService.Verify(ws => ws.FetchWeatherDataAsync(It.IsAny<string>()), Times.Once);
            _mockBlobService.Verify(bs => bs.SavePayloadToBlobAsync(testData, It.IsAny<string>()), Times.Once);
            _mockTableService.Verify(ts => ts.SaveLogToTableAsync(It.IsAny<WeatherFunctionApp.Core.Models.WeatherLogEntity>()), Times.Once);
        }

        [Fact]
        public async Task FetchWeatherData_Failure()
        {
            // Arrange
            string errorMessage = "API call failed";
            _mockWeatherService.Setup(ws => ws.FetchWeatherDataAsync(It.IsAny<string>()))
                               .ThrowsAsync(new HttpRequestException(errorMessage));
            var function = new FetchWeatherData(_mockWeatherService.Object, _mockBlobService.Object, _mockTableService.Object);

            // Act
            await function.Run(null, _mockLogger.Object);

            // Assert
            _mockWeatherService.Verify(ws => ws.FetchWeatherDataAsync(It.IsAny<string>()), Times.Once);
            _mockTableService.Verify(ts => ts.SaveLogToTableAsync(It.Is<WeatherFunctionApp.Core.Models.WeatherLogEntity>(e => e.Status == "Failure" && e.Message == errorMessage)), Times.Once);
        }
    }
}
