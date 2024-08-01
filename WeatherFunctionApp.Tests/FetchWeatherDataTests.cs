using Microsoft.Extensions.Logging;
using Moq;
using WeatherFunctionApp.Core.Interfaces;

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
            _mockTableService.Verify(ts => ts.SaveLogToTableAsync(
                It.Is<string>(pk => pk == "WeatherLog"),
                It.IsAny<string>(),
                It.IsAny<DateTimeOffset>(),
                It.Is<string>(status => status == "Success"),
                It.IsAny<string>()),
                Times.Once
                );
        }

        [Fact]
        public async Task FetchWeatherData_Failure()
        {
            // Arrange
            string errorMessage = "API call failed";
            _mockWeatherService.Setup(ws => ws.FetchWeatherDataAsync(It.IsAny<string>()))
                               .ThrowsAsync(new HttpRequestException(errorMessage));
            var function = new FetchWeatherData(_mockWeatherService.Object, _mockBlobService.Object, _mockTableService.Object);;

            // Act
            await function.Run(null, _mockLogger.Object);

            // Assert
            _mockWeatherService.Verify(ws => ws.FetchWeatherDataAsync(It.IsAny<string>()), Times.Once);
            _mockTableService.Verify(ts => ts.SaveLogToTableAsync(
                It.Is<string>(pk => pk == "WeatherLog"),
                It.IsAny<string>(),
                It.IsAny<DateTimeOffset>(),
                It.Is<string>(status => status == "Failure"),
                It.Is<string>(message => message == errorMessage)),
                Times.Once
                );
        }
    }
}
