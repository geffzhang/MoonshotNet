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
