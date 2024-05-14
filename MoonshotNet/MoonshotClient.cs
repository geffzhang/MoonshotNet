using MoonshotNet.ChatCompletions;
using MoonshotNet.Serialization;
using MoonshotNet.Sse;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoonshotNet;

public sealed class MoonshotClient
{
    private readonly HttpClient client;
    private readonly ILogger logger;
    private readonly MoonshotModel model;

    private const string MoonshotApiVersion = "1";
    private const string MoonshotEndpoint = $"https://api.moonshot.cn/v{MoonshotApiVersion}/";

    public static JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = {
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower),
            new JsonMoonshotChatRoleConverter(),
            new JsonMoonshotModelConverter()
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public MoonshotClient(string apiKey, MoonshotModel model, HttpClient? httpClient = null, ILogger<MoonshotClient>? logger = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(apiKey, nameof(apiKey));
        ArgumentException.ThrowIfNullOrEmpty(model.Value, nameof(model));

        this.model = model;

        client = httpClient ?? new HttpClient();
        client.BaseAddress = new Uri(MoonshotEndpoint);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        this.logger = logger ?? NullLogger<MoonshotClient>.Instance;
    }

    public async Task<MoonshotChatCompletions> GetChatCompletionsAsync(
        IList<MoonshotMessage> messages,
        MoonshotChatCompletionOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages, nameof(messages));

        options ??= new MoonshotChatCompletionOptions();

        var request = new MoonshotChatCompletionsRequest
        {
            Model = model,
            Messages = messages,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            Stop = options.Stop,
            Stream = false,
            //ResponseFormat = options.ResponseFormat,
            Tools = options.Tools
        };

        var response = await GetResponseAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStreamAsync(cancellationToken);

        var result = await JsonSerializer.DeserializeAsync<MoonshotChatCompletions>(content, SerializerOptions, cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize Moonshot response.");
        }

        return result;
    }

    public async IAsyncEnumerable<StreamingUpdate> GetChatCompletionsStreamingAsync(
        IList<MoonshotMessage> messages,
        MoonshotChatCompletionOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages, nameof(messages));

        options ??= new MoonshotChatCompletionOptions();

        var request = new MoonshotChatCompletionsRequest
        {
            Model = model,
            Messages = messages,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            Stop = options.Stop,
            Stream = true,
            Tools = options.Tools
        };

        var response = await GetResponseAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var stream = SseAsyncEnumerator<StreamingUpdate>.EnumerateFromSseStream(
            content, 
            e => JsonSerializer.Deserialize<StreamingUpdate>(e, SerializerOptions), 
            cancellationToken);

        await foreach (var item in stream)
        {
            yield return item;
        }
    }

    private async Task<HttpResponseMessage> GetResponseAsync(MoonshotChatCompletionsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        int retryAfterSeconds = 0;

        try
        {
            var jsonContent = JsonSerializer.Serialize(request, SerializerOptions);
           
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("chat/completions", httpContent, cancellationToken)
                                       .ConfigureAwait(false);

            if (response == null)
            {
                throw new InvalidOperationException("The Moonshot response is null.");
            }

            retryAfterSeconds = GetRetryAfterSeconds(response);
            response.EnsureSuccessStatusCode();

            return response;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            logger?.LogError($"Rate limit reached, sleeping for {retryAfterSeconds} seconds.");

            // Sleep for the number of seconds specified by the server
            await Task.Delay(retryAfterSeconds * 1000, cancellationToken);

            // Retry the request
            return await GetResponseAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve Moonshot completions.", ex);
        }
    }

    private int GetRetryAfterSeconds(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            if (response.Headers.TryGetValues("retry-after", out var retryAfterValues))
            {
                return int.Parse(retryAfterValues?.FirstOrDefault() ?? "0");
            }
        }

        return 0;
    }

}
