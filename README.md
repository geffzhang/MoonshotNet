# Moonshot .NET Client Service

[![NuGet version (MoonshotNet)](https://img.shields.io/nuget/v/MoonshotNet.svg?style=flat-square)](https://www.nuget.org/packages/MoonshotNet/)
[![NuGet downloads (MoonshotNet)](https://img.shields.io/nuget/dt/MoonshotNet.svg?style=flat-square)](https://www.nuget.org/packages/MoonshotNet/)
[![GitHub license](https://img.shields.io/github/license/geffzhang/MoonshotNet)](https://github.com/geffzhang/MoonshotNet)

The Moonshot .NET is a library for interacting with the Moonshot API, the quickest LLM inference method available yet. 

## About

The library provides a simple and easy-to-use interface for accessing the Moonshot API, allowing developers to integrate the platform's capabilities into their applications.

## Features

* Sends HTTP requests to the Moonshot API
* Streaming chat completions
* Handles rate limiting and retries when necessary
* Supports JSON serialization and deserialization using System.Text.Json
* Can be used with ILogger for logging

## Usage

Here's an example of how to use the client library:

```csharp
using MoonshotNet;
using MoonshotNet.ChatCompletions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var apiKey = Environment.GetEnvironmentVariable("API_Key_Moonshot", EnvironmentVariableTarget.User);

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddMoonshotClient(apiKey, MoonshotModel.Moonshot_v1_8kValue);
    }).Build();

var moonshotClient = host.Services.GetRequiredService<MoonshotClient>();

var history = new MoonshotChatHistory
{
    new("���� Kimi���� Moonshot AI �ṩ���˹��������֣�����ó����ĺ�Ӣ�ĵĶԻ������Ϊ�û��ṩ��ȫ���а�����׼ȷ�Ļش�ͬʱ�����ܾ�һ���漰�ֲ����壬�������ӣ���ɫ����������Ļش�Moonshot AI Ϊר�����ʣ����ɷ������������?")
};

// -- Example 1: Get chat completions without streaming
var result = await moonshotClient.GetChatCompletionsAsync(history);

Console.WriteLine(result.Choices.First().Message.Content);
Console.WriteLine($"Total tokens used: {result.Usage.TotalTokens}");

// -- Example 2: Get chat completions with streaming
await foreach (var msg in moonshotClient.GetChatCompletionsStreamingAsync(history))
{
    Console.WriteLine(msg.Choices[0].Delta.Content);
}
```

## License

The Moonshot .NET Client Library is licensed under the Apache 2.0 License.

## Contribution

Contributions are welcome! If you find a bug or have an idea for a new feature, please open an issue and let us know.
