using System.Text.Json;
using System.Text.Json.Serialization;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Converters
{
    internal class SaveLocationJsonConverter : JsonConverter<SaveLocation>
    {
        public override SaveLocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return SaveLocation.Create(reader.GetString());
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return SaveLocation.Create(reader.GetInt32());
            }

            throw new JsonException($"Unsupported token type {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, SaveLocation value, JsonSerializerOptions options)
        {
            var serializedValue = value.ToValue();
            if (serializedValue is int intValue)
            {
                writer.WriteNumberValue(intValue);
            }
            else
            {
                writer.WriteStringValue((string)serializedValue);
            }
        }
    }
}
