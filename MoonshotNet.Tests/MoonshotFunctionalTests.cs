using MoonshotNet.ChatCompletions;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Xunit;

namespace MoonshotNet.Tests
{
    public class MoonshotFunctionalTests
    {
        [Fact]
        public async Task GetChatCompletionsAsync_NoApiKey()
        {
            // Arrange
            var apiKey = "NOKEY";

            using var httpClient = new HttpClient();
            var logger = new Logger<MoonshotClient>(new LoggerFactory());
            var cancellationToken = new CancellationToken();
            var options = new MoonshotChatCompletionOptions(1.0m, 100, 1.0m);
            var model = MoonshotModel.Moonshot_v1_8k;
            var conversation = new MoonshotChatHistory
            {
                new("What is the capital of France?")
            };

            try
            {
                // Act
                var client = new MoonshotClient(apiKey, model, httpClient, logger);
                await client.GetChatCompletionsAsync(conversation, options, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                // Assert
                Assert.IsType<HttpRequestException>(ex.InnerException);
                Assert.Equal(HttpStatusCode.Unauthorized, ((HttpRequestException)ex.InnerException).StatusCode);
            }
        }

        [Fact]
        public async Task GetChatCompletionsAsync_NonStreaming()
        {
            // Arrange
            var apiKey = Environment.GetEnvironmentVariable("API_Key_Moonshot", EnvironmentVariableTarget.User);
            var model = MoonshotModel.Moonshot_v1_8k;
            using var httpClient = new HttpClient();
            var logger = new Logger<MoonshotClient>(new LoggerFactory());
            var cancellationToken = new CancellationToken();
            var options = new MoonshotChatCompletionOptions(1.0m, 1024, 1.0m);
            var conversation = new MoonshotChatHistory
            {
                new("What is the capital of France?")
            };

            // Act
            var client = new MoonshotClient(apiKey, model, httpClient, logger);
            var response = await client.GetChatCompletionsAsync(conversation, options, cancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Choices.Count > 0);
            Assert.Equal(MoonshotChatRole.Assistant, response.Choices[0].Message.Role);
        }

        [Fact]
        public async Task GetChatCompletionsToolUseAsync()
        {
            // Arrange
            var apiKey = Environment.GetEnvironmentVariable("API_Key_Moonshot", EnvironmentVariableTarget.User);
            var model = MoonshotModel.Moonshot_v1_8k;
            using var httpClient = new HttpClient();
            var logger = new Logger<MoonshotClient>(new LoggerFactory());
            var cancellationToken = new CancellationToken();
            var tools = new List<FunctionTool>();
            tools.Add(ConvertToFunctionTool());


            var options = new MoonshotChatCompletionOptions(1.0m, 1024, 1.0m, tools: tools);
            var conversation = new MoonshotChatHistory
            {
                new(role:"user","编程判断 3214567 是否是素数")
            };

            // Act
            var client = new MoonshotClient(apiKey, model, httpClient, logger);
            var response = await client.GetChatCompletionsAsync(conversation, options, cancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Choices.Count > 0);
            Assert.Equal(MoonshotChatRole.Assistant, response.Choices[0].Message.Role);
            Assert.Equal("tool_calls", response.Choices[0].FinishReason);
        }

        private static FunctionTool ConvertToFunctionTool()
        {
            var funcTools = new FunctionTool().SetName("CodeRunner").SetDescription("代码执行器，支持运行 python 和 javascript 代码");
            funcTools.SetParameters(JsonDocument.Parse("""
                     {
                  "properties": {
                    "language": {
                      "type": "string",
                      "enum": ["python", "javascript"]
                    },
                    "code": {
                      "type": "string",
                      "description": "代码写在这里"
                    }
                  },
                  "type": "object"
                }
                """));
            return funcTools;
        }
    }


}

public class FunctionParameters
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    /// <summary>
    /// ParameterPropertyDef
    /// {
    ///     "field_name": {}
    /// }
    /// </summary>
    [JsonPropertyName("properties")]
    public JsonDocument Properties { get; set; } = JsonSerializer.Deserialize<JsonDocument>("{}");

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>();

    public override string ToString()
    {
        return $"{{\"type\":\"{Type}\", \"properties\":{JsonSerializer.Serialize(Properties)}, \"required\":[{string.Join(",", Required.Select(x => "\"" + x + "\""))}]}}";
    }

    public FunctionParameters()
    {

    }
}