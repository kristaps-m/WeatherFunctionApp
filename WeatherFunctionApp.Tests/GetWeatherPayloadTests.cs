using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherFunctionApp.Core.Interfaces;

namespace WeatherFunctionApp.Tests
{
    public class GetWeatherPayloadTests
    {
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<ILogger> _mockLogger;

        public GetWeatherPayloadTests()
        {
            _mockBlobService = new Mock<IBlobService>();
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public async Task GetPayloadByLogId_ReturnsPayload()
        {
            // Arrange
            string testData = "{\"weather\":\"clear\"}";
            _mockBlobService.Setup(bs => bs.GetPayloadFromBlobAsync(It.IsAny<string>()))
                            .ReturnsAsync(testData);

            var context = new DefaultHttpContext();
            var function = new GetWeatherPayload(_mockBlobService.Object);

            // Act
            var result = await function.Run(context.Request, "logId", _mockLogger.Object) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData, result.Value);
        }

        [Fact]
        public async Task GetPayloadByLogId_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockBlobService.Setup(bs => bs.GetPayloadFromBlobAsync(It.IsAny<string>()))
                            .ReturnsAsync((string)null);

            var context = new DefaultHttpContext();
            var function = new GetWeatherPayload(_mockBlobService.Object);

            // Act
            var result = await function.Run(context.Request, "logId", _mockLogger.Object) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Log not found.", result.Value);
        }
    }
}
