using QBittorrent.ApiClient.Converters;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a node in the qBittorrent RSS item tree.
    /// </summary>
    [JsonConverter(typeof(RssItemJsonConverter))]
    public abstract record RssItem
    {
    }
}
