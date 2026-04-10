using System.Text.Json;
using System.Text.Json.Serialization;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Converters
{
    internal sealed class DirectoryContentEntryTypeJsonConverter : JsonConverter<DirectoryContentEntryType>
    {
        public override DirectoryContentEntryType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            return value switch
            {
                "dir" => DirectoryContentEntryType.Directory,
                "file" => DirectoryContentEntryType.File,
                _ => throw new JsonException($"Unsupported directory content entry type '{value}'.")
            };
        }

        public override void Write(Utf8JsonWriter writer, DirectoryContentEntryType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value switch
            {
                DirectoryContentEntryType.Directory => "dir",
                DirectoryContentEntryType.File => "file",
                _ => throw new JsonException($"Unsupported directory content entry type '{value}'.")
            });
        }
    }
}
