using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Converters
{
    internal sealed class NullableStringDoubleJsonConverter : JsonConverter<double?>
    {
        public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.String => ParseString(reader.GetString()),
                JsonTokenType.Number => reader.GetDouble(),
                _ => null
            };
        }

        public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteNumberValue(value.Value);
        }

        private static double? ParseString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || (value == "-"))
            {
                return null;
            }

            return double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : null;
        }
    }
}
