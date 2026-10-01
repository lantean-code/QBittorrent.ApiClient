using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Converters
{
    internal sealed class StringOrNumberJsonConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            if ((reader.TokenType == JsonTokenType.Number) && reader.TryGetInt64(out var value))
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            throw new JsonException("Expected a string, integer, or null JSON value.");
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
