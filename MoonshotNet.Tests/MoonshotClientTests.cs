using MoonshotNet.ChatCompletions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Diagnostics;
using System.Net;
using Xunit;

namespace MoonshotNet.Tests
{
    public class MoonshotClientTests
    {
        [Fact]
        public void Constructor_NoKey_Throws_ArgumentNullException()
        {
            // Arrange
            string? apiKey = null;
            var model = MoonshotModel.Moonshot_v1_8k;
            var messageHandlerStub = new HttpMessageHandlerStub();
            var httpClient = new HttpClient(messageHandlerStub, false);
            var logger = new Mock<ILogger<MoonshotClient>>();

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new MoonshotClient(apiKey, model, httpClient, logger.Object));
        }

        [Fact]
        public void Constructor_NoModel_Throws_ArgumentNullException()
        {
            // Arrange
            var apiKey = "NOKEY";
            MoonshotModel model;
            var messageHandlerStub = new HttpMessageHandlerStub();
            var httpClient = new HttpClient(messageHandlerStub, false);
            var logger = new Mock<ILogger<MoonshotClient>>();


            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new MoonshotClient(apiKey, model, httpClient, logger.Object));
        }

        [Fact]
        public void Constructor_NoHttpClient_Success()
        {
            // Arrange
            var apiKey = "NOKEY";
            var model = MoonshotModel.Moonshot_v1_8k;
            HttpClient? httpClient = null;
            var logger = new Mock<ILogger<MoonshotClient>>();

            // Act
            var client = new MoonshotClient(apiKey, model, httpClient, logger.Object);

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public void Constructor_NoLogger_Success()
        {
            // Arrange
            var apiKey = "NOKEY";
            var model = MoonshotModel.Moonshot_v1_8k;
            var messageHandlerStub = new HttpMessageHandlerStub();
            var httpClient = new HttpClient(messageHandlerStub, false);
            ILogger<MoonshotClient>? logger = null;

            // Act
            var client = new MoonshotClient(apiKey, model, httpClient, logger);

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public async Task GetChatCompletionsAsync_NoConversation_Throws_ArgumentNullException()
        {
            // Arrange
            var apiKey = "NOKEY";
            var model = MoonshotModel.Moonshot_v1_8k;
            var messageHandlerStub = new HttpMessageHandlerStub();
            var httpClient = new HttpClient(messageHandlerStub, false);
            var options = new Mock<MoonshotChatCompletionOptions>();
            var logger = new Mock<ILogger<MoonshotClient>>();

            IList<MoonshotMessage>? conversation = null;

            // Act
            var client = new MoonshotClient(apiKey, model, httpClient, logger.Object);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetChatCompletionsAsync(conversation, options.Object));
        }

        [Fact]
        public async Task GetChatCompletionsAsync_ValidRequest_ReturnsMoonshotResponse()
        {
            // Arrange
            var apiKey = "NOKEY";
            var model = MoonshotModel.Moonshot_v1_8k;
            var conversation = new MoonshotChatHistory { new("Hello") };

            var fileContent = File.ReadAllText("data/response.json");
            var messageHandlerStub = new HttpMessageHandlerStub();
            messageHandlerStub.ResponseToReturn = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(fileContent) };
            var httpClient = new HttpClient(messageHandlerStub, false);
            var options = new Mock<MoonshotChatCompletionOptions>();
            var logger = new Mock<ILogger<MoonshotClient>>();

            // Act
            var client = new MoonshotClient(apiKey, model, httpClient, logger.Object);
            var response = await client.GetChatCompletionsAsync(conversation, options.Object);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("Hello World", response.Choices[0].Message.Content);
        }

        [Fact]
        public async Task GetChatCompletionsAsync_TooManyRequests_RetriesAfterDelay()
        {
            // Arrange
            var apiKey = "NOKEY";
            var model = MoonshotModel.Moonshot_v1_8k;
            var conversation = new MoonshotChatHistory { new("Hello") };

            var fileContent = File.ReadAllText("data/response.json");
            var httpResponse1 = new HttpResponseMessage { StatusCode = HttpStatusCode.TooManyRequests, Content = new StringContent(fileContent) };
            httpResponse1.Headers.Add("retry-after", "1");
            var httpResponse2 = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(fileContent) };
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            messageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse1)
                .ReturnsAsync(httpResponse2);
            var httpClient = new HttpClient(messageHandlerMock.Object, false);
            var options = new Mock<MoonshotChatCompletionOptions>();
            var logger = new Mock<ILogger<MoonshotClient>>();
            var sw = new Stopwatch();

            // Act
            var client = new MoonshotClient(apiKey, model, httpClient, logger.Object);

            sw.Start();
            var response = await client.GetChatCompletionsAsync(conversation, options.Object);
            sw.Stop();

            // Assert
            Assert.NotNull(response);
            Assert.True(sw.ElapsedMilliseconds >= 1000);
            Assert.Equal("Hello World", response.Choices[0].Message.Content);
        }
    }
}