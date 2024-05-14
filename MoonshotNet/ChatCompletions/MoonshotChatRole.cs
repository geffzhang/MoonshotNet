namespace MoonshotNet.ChatCompletions;

public readonly struct MoonshotChatRole : IEquatable<MoonshotChatRole>
{
    private readonly string _value;

    public MoonshotChatRole(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private const string SystemValue = "system";
    private const string AssistantValue = "assistant";
    private const string UserValue = "user";

    public static MoonshotChatRole System { get; } = new MoonshotChatRole(SystemValue);

    public static MoonshotChatRole Assistant { get; } = new MoonshotChatRole(AssistantValue);

    public static MoonshotChatRole User { get; } = new MoonshotChatRole(UserValue);


    public static bool operator ==(MoonshotChatRole left, MoonshotChatRole right) => left.Equals(right);
    public static bool operator !=(MoonshotChatRole left, MoonshotChatRole right) => !left.Equals(right);
    public static implicit operator MoonshotChatRole(string value) => new MoonshotChatRole(value);
    public override bool Equals(object obj) => obj is MoonshotChatRole other && Equals(other);
    public bool Equals(MoonshotChatRole other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    public override string ToString() => _value;
}