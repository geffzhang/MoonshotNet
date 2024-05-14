using System.Text.Json;
using MoonshotNet.ChatCompletions;
using Xunit;

namespace MoonshotNet.Tests
{
    public class MoonshotSerializationTests
    {
        [Fact]
        public async Task SerializeAsync_MoonshotRequest_ReturnsJsonString()
        {
            // Arrange
            var request = new MoonshotChatCompletionsRequest
            {
                Model = MoonshotModel.Moonshot_v1_8k,
                Messages = new MoonshotMessage[]
                {
                    new("Hello, how are you?")
                    {
                        Name="geffzhang"
                    }
                },
                Temperature = 0.5M,
                TopP = 0.9M,
                MaxTokens = 100,
                Stream = false
            };

            // Act
            using var jsonStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(jsonStream, request, MoonshotClient.SerializerOptions);

            // Assert
            jsonStream.Position = 0;
            var result = new StreamReader(jsonStream).ReadToEnd();

            Assert.Equal(@"
{""model"":""moonshot-v1-8k"",
""messages"":[{
""role"":""user"",
""content"":""Hello, how are you?"",
""name"":""geffzhang""
}],
""temperature"":0.5,
""max_tokens"":100,
""top_p"":0.9,
""stream"":false
}".Replace("\r\n", ""), result);
        }

        [Fact]
        public async Task DeserializeAsync_JsonString_Returns_MoonshotRequest()
        {
            // Arrange
            var responseContent = File.ReadAllText("data/response.json");
            var expected = new MoonshotChatCompletions
            {
                Id = "34a9110d-c39d-423b-9ab9-9c748747b204",
                Object = "chat.completion",
                Created = 1708045122,
                Model = "moonshot-v1-32k",
                Choices = new List<MoonshotChoice>
                {
                    new ()
                    {
                        Index = 0,
                        Message = new MoonshotMessage(MoonshotChatRole.Assistant, "Hello World"),
                        FinishReason = "stop"
                    }
                },
                Usage = new MoonshotUsage
                {
                    PromptTokens = 24,
                    CompletionTokens = 377,
                    TotalTokens = 401,
                 }
            };

            // Act
            using var jsonStream = new MemoryStream();
            using var stream = new StreamWriter(jsonStream);
            stream.Write(responseContent);
            stream.Flush();
            jsonStream.Position = 0;
            var actual = await JsonSerializer.DeserializeAsync<MoonshotChatCompletions>(jsonStream, MoonshotClient.SerializerOptions);

            // Assert

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Object, actual.Object);
            Assert.Equal(expected.Created, actual.Created);
            Assert.Equal(expected.Model, actual.Model);
            Assert.Equal(expected.Choices.Count, actual.Choices.Count);
            Assert.Equal(expected.Choices[0].Index, actual.Choices[0].Index);
            Assert.Equal(expected.Choices[0].FinishReason, actual.Choices[0].FinishReason);
            Assert.Equal(expected.Choices[0].Logprobs, actual.Choices[0].Logprobs);
            Assert.Equal(expected.Choices[0].Message.Role, actual.Choices[0].Message.Role);
            Assert.Equal(expected.Choices[0].Message.Content, actual.Choices[0].Message.Content);
            Assert.Equal(expected.Usage.PromptTokens, actual.Usage.PromptTokens);
            Assert.Equal(expected.Usage.CompletionTokens, actual.Usage.CompletionTokens);
            Assert.Equal(expected.Usage.TotalTokens, actual.Usage.TotalTokens);

        }

        [Fact]
        public async Task DeserializeAsyncEnumerable_JsonStream_Returns_MoonshotRequest()
        {
            // Arrange
            var responseContent = File.ReadAllText("data/response.json");
            var expected = new MoonshotChatCompletions
            {
                Id = "34a9110d-c39d-423b-9ab9-9c748747b204",
                Object = "chat.completion",
                Created = 1708045122,
                Model = "moonshot-v1-32k",
                Choices = new List<MoonshotChoice>
                {
                    new ()
                    {
                        Index = 0,
                        Message = new MoonshotMessage(MoonshotChatRole.Assistant,"Hello World"),
                        FinishReason = "stop",
                        Logprobs = null
                    }
                },
                Usage = new MoonshotUsage
                {
                    PromptTokens = 24,
                    CompletionTokens = 377,
                    TotalTokens = 401
                }
            };

            // Act
            using var jsonStream = new MemoryStream();
            using var stream = new StreamWriter(jsonStream);
            stream.Write($"[{responseContent}]");
            stream.Flush();
            jsonStream.Position = 0;

            await foreach (var actual in JsonSerializer.DeserializeAsyncEnumerable<MoonshotChatCompletions>(jsonStream, MoonshotClient.SerializerOptions))
            {
                // Assert
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Object, actual.Object);
                Assert.Equal(expected.Created, actual.Created);
                Assert.Equal(expected.Model, actual.Model);
                Assert.Equal(expected.Choices.Count, actual.Choices.Count);
                Assert.Equal(expected.Choices[0].Index, actual.Choices[0].Index);
                Assert.Equal(expected.Choices[0].FinishReason, actual.Choices[0].FinishReason);
                Assert.Equal(expected.Choices[0].Logprobs, actual.Choices[0].Logprobs);
                Assert.Equal(expected.Choices[0].Message.Role, actual.Choices[0].Message.Role);
                Assert.Equal(expected.Choices[0].Message.Content, actual.Choices[0].Message.Content);
                Assert.Equal(expected.Usage.PromptTokens, actual.Usage.PromptTokens);
                Assert.Equal(expected.Usage.CompletionTokens, actual.Usage.CompletionTokens);
                Assert.Equal(expected.Usage.TotalTokens, actual.Usage.TotalTokens);

            }
        }
    }
}
