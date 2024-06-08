using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonshotNet.ChatCompletions
{
    public class ChatCompletionsToolCall
    {
        public required int Index { get; set; }

        public required string Id { get; set; }

        public string type { get; set; } = "function";

        public InternalChatCompletionMessageToolCallFunction Function { get; set; } = new();
    }

    public class InternalChatCompletionMessageToolCallFunction
    {
        public string Name { get; set; }

        public string Arguments { get; set; }

        internal IDictionary<string, BinaryData> _serializedAdditionalRawData;

        public InternalChatCompletionMessageToolCallFunction(string name, string arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        internal InternalChatCompletionMessageToolCallFunction(string name, string arguments, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            Name = name;
            Arguments = arguments;
            _serializedAdditionalRawData = serializedAdditionalRawData;
        }

        internal InternalChatCompletionMessageToolCallFunction()
        {

        }
    }
}
