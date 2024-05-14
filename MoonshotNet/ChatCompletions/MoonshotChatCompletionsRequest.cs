namespace MoonshotNet.ChatCompletions;

public class MoonshotChatCompletionsRequest
{
    public required MoonshotModel Model { get; set; }

    public required IList<MoonshotMessage> Messages { get; set; }

    public decimal Temperature { get; set; }

    public int MaxTokens { get; set; }

    public decimal TopP { get; set; }

    public bool Stream { get; set; } = false;

    public string? Stop { get; set; }

    public IList<FunctionTool>? Tools { get; set; }

    public MoonshotToolChoice? ToolChoice { get; set; }
}
