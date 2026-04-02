using QBittorrent.ApiClient.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient
{
    internal static class SerializerOptions
    {
        private static readonly JsonSerializerOptions _defaultOptions = CreateDefaultOptions();

        internal static JsonSerializerOptions Options
        {
            get { return new JsonSerializerOptions(_defaultOptions); }
        }

        private static JsonSerializerOptions CreateDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new DirectoryContentEntryTypeJsonConverter());
            options.Converters.Add(new SaveLocationJsonConverter());

            return options;
        }
    }
}
