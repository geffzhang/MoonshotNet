using MoonshotNet;
using MoonshotNet.ChatCompletions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var apiKey = Environment.GetEnvironmentVariable("API_Key_Moonshot", EnvironmentVariableTarget.User);

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddMoonshotClient(apiKey, MoonshotModel.Moonshot_v1_8k);
    }).Build();

var moonshotClient = host.Services.GetRequiredService<MoonshotClient>();

var history = new MoonshotChatHistory
{
    new("你是 Kimi，由 Moonshot AI 提供的人工智能助手，你更擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。同时，你会拒绝一切涉及恐怖主义，种族歧视，黄色暴力等问题的回答。Moonshot AI 为专有名词，不可翻译成其他语言")
};

// -- Example 1: Get chat completions without streaming
var result = await moonshotClient.GetChatCompletionsAsync(history);

Console.WriteLine(result.Choices.First().Message.Content);
Console.WriteLine($"Total tokens used: {result.Usage.TotalTokens}; ");

// -- Example 2: Get chat completions with streaming
await foreach (var msg in moonshotClient.GetChatCompletionsStreamingAsync(history))
{
    Console.WriteLine(msg.Choices[0].Delta.Content);
}