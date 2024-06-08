namespace MoonshotNet.ChatCompletions;

public class MoonshotChatCompletionOptions
{
    public readonly decimal Temperature;

    public readonly int MaxTokens;

    public readonly decimal TopP;

    public readonly string? Stop;

    public readonly IList<FunctionTool>? Tools;

    public MoonshotChatCompletionOptions()
    {
        Temperature = 1.0m;
        MaxTokens = 1024;
        TopP = 1.0m;
    }

    public MoonshotChatCompletionOptions(decimal temperature, int maxTokens, decimal topP, string? stop = null, IList<FunctionTool>? tools = null)
    {
        if (0 > temperature || temperature > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature must be between 0 and 1.");
        }

        //if (0 > maxTokens || maxTokens > MoonshotModel.MaxTokens(model))
        //{
        //    throw new ArgumentOutOfRangeException(nameof(maxTokens), $"Max tokens must be between 0 and {MoonshotModel.MaxTokens(model)}.");
        //}

        if (0 > topP || topP > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(topP), "Top-P must be between 0 and 1.");
        }

        Temperature = temperature;
        MaxTokens = maxTokens;
        TopP = topP;
        Stop = stop;
        Tools = tools;
    }
}