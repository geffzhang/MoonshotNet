namespace MoonshotNet.ChatCompletions;

public class MoonshotChoice
{
    public required int Index { get; set; }

    public required MoonshotMessage Message { get; set; }

    public required string FinishReason { get; set; }

    public object? Logprobs { get; set; }
}