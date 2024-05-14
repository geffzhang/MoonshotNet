using MoonshotNet.ChatCompletions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoonshotNet.Serialization
{
    public class JsonMoonshotChatRoleConverter : JsonConverter<MoonshotChatRole>
    {
        public override MoonshotChatRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var role = reader.GetString();

            switch (role)
            {
                case "user":
                    return MoonshotChatRole.User;
                case "assistant":
                    return MoonshotChatRole.Assistant;
                case "system":
                    return MoonshotChatRole.System;
                default:
                    throw new JsonException($"Unknown chat role: '{role}'");
            }
        }

        public override void Write(Utf8JsonWriter writer, MoonshotChatRole role, JsonSerializerOptions options)
        {
            writer.WriteStringValue(role.ToString());
        }
    }
}
