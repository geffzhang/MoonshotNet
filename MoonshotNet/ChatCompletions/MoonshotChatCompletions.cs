namespace MoonshotNet.ChatCompletions;

public class MoonshotChatCompletions
{
    public required string Id { get; set; }

    public required string Object { get; set; }

    public required long Created { get; set; }

    public required string Model { get; set; }

    public required IReadOnlyList<MoonshotChoice> Choices { get; set; }

    public required MoonshotUsage Usage { get; set; }
}