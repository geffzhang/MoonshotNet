namespace MoonshotNet.ChatCompletions;

/// <summary>
/// Represents pre-trained models provided by Moonshot with identifiers and additional metadata.
/// </summary>
public readonly struct MoonshotModel : IEquatable<MoonshotModel>
{
    private readonly string _value;

    public MoonshotModel(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private const string Moonshot_v1_8kValue = "moonshot-v1-8k";
    private const string Moonshot_v1_32kValue = "moonshot-v1-32k";
    private const string Moonshot_v1_128kValue = "moonshot-v1-128k";

    /// <summary>
    /// Model ID: Moonshot_v1_8k
    /// </summary>
    public static MoonshotModel Moonshot_v1_8k { get; } = new MoonshotModel(Moonshot_v1_8kValue);

    /// <summary>
    /// Model ID: Moonshot_v1_32k
    /// </summary>
    public static MoonshotModel Moonshot_v1_32k { get; } = new MoonshotModel(Moonshot_v1_32kValue);

    /// <summary>
    /// Model ID: Moonshot_v1_128k
    /// </summary>
    public static MoonshotModel Moonshot_v1_128k { get; } = new MoonshotModel(Moonshot_v1_128kValue);

    public static int MaxTokens(MoonshotModel model)
    {
        return model._value switch
        {
            Moonshot_v1_8kValue => 8192,
            Moonshot_v1_32kValue => 32768,
            Moonshot_v1_128kValue => 131072,
            _ => 8192 // Unknown model -> defaults to 8192
        };
    }

    public bool Equals(MoonshotModel other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => _value;

    public string Value { get { return _value; } }
}
