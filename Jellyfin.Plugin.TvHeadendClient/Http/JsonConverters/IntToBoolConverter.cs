using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.Http.JsonConverters;

public class IntToBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => Convert.ToBoolean(reader.GetInt32()),
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Null => null,
            _ => throw new JsonException($"Unexpected token parsing bool?. Got {reader.TokenType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
