using System.Text.Json;

namespace MoonshotNet.ChatCompletions;

public class FunctionTool
{
    public string type { get; set; } = "function";

    public Dictionary<string, object> function { get; set; } = new();

    public FunctionTool SetName(string name)
    {
        function["name"] = name;
        return this;
    }

    public FunctionTool SetDescription(string desc)
    {
        function["description"] = desc;
        return this;
    }

    public FunctionTool SetParameters(JsonDocument param)
    {
        function["parameters"] = param;
        return this;
    }
}