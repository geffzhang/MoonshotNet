namespace MoonshotNet.ChatCompletions;

public class MoonshotMessage
{
    /// <summary>
    /// The role of the chat participant.
    /// </summary>
    public MoonshotChatRole Role { get; set; }

    /// <summary>
    ///  The text of a message.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The tool calls that must be resolved and have their outputs appended to subsequent input messages for the chat
    /// completions request to resolve as configured.
    /// Please note <see cref="ChatCompletionsToolCall"/> is the base class. According to the scenario, a derived class of the base class might need to be assigned here, or this property needs to be casted to one of the possible derived classes.
    /// The available derived classes include <see cref="FunctionTool"/>.
    /// </summary>
    public List<ChatCompletionsToolCall> ToolCalls { get; set; }

    /// <summary>
    /// An optional name to disambiguate messages from different users with the same role.
    /// </summary>
    public string? Name { get; set; }

    public MoonshotMessage() { }

    public MoonshotMessage(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

        Role = MoonshotChatRole.User;
        Content = content;        
    }

    public MoonshotMessage(MoonshotChatRole role, string content, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

        Role = role;
        Content = content;
        Name = name;
    }
}
