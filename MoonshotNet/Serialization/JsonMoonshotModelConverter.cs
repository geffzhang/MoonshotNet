using MoonshotNet.ChatCompletions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoonshotNet.Serialization
{
    public class JsonMoonshotModelConverter : JsonConverter<MoonshotModel>
    {
        public override MoonshotModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = reader.GetString();

            switch (model)
            {
                case "moonshot-v1-8k":
                    return MoonshotModel.Moonshot_v1_8k;
                case "moonshot-v1-32k":
                    return MoonshotModel.Moonshot_v1_32k;
                case "moonshot-v1-128k":
                    return MoonshotModel.Moonshot_v1_128k;                
                default:
                    throw new JsonException($"Unknown model: '{model}'");
            }
        }

        public override void Write(Utf8JsonWriter writer, MoonshotModel model, JsonSerializerOptions options)
        {
            writer.WriteStringValue(model.ToString());
        }
    }
}
