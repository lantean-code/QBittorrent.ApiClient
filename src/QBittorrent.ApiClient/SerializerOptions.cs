using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace QBittorrent.ApiClient
{
    internal static class SerializerOptions
    {
        private static readonly QBittorrentJsonSerializerContext _context;
        private static readonly JsonSerializerOptions _defaultOptions;

        static SerializerOptions()
        {
            _defaultOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _context = new QBittorrentJsonSerializerContext(_defaultOptions);
        }

        internal static JsonSerializerOptions Options
        {
            get { return new JsonSerializerOptions(_context.Options); }
        }

        internal static QBittorrentJsonSerializerContext Context
        {
            get { return _context; }
        }

        internal static JsonTypeInfo<T> GetTypeInfo<T>()
        {
            return (JsonTypeInfo<T>)_context.GetTypeInfo(typeof(T))!;
        }
    }
}
