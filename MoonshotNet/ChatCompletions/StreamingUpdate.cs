﻿namespace MoonshotNet.ChatCompletions;

public class StreamingUpdate
{
    public required string Id { get; set; }

    public required string Object { get; set; }

    public required long Created { get; set; }

    public required string Model { get; set; }

    public required IReadOnlyList<StreamingChoice> Choices { get; set; }
}