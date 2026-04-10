using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    internal sealed record SearchStartResult
    {
        [JsonConstructor]
        public SearchStartResult(int? id)
        {
            Id = id;
        }

        [JsonPropertyName("id")]
        public int? Id { get; }
    }
}
