using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the accepted-but-not-yet-resolved payload returned while torrent metadata is still being fetched.
    /// </summary>
    public record FetchTorrentMetadataPendingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchTorrentMetadataPendingResult" /> class.
        /// </summary>
        /// <param name="infoHashV1">The v1 info hash when available.</param>
        /// <param name="infoHashV2">The v2 info hash when available.</param>
        /// <param name="hash">The qBittorrent torrent hash when available.</param>
        [JsonConstructor]
        public FetchTorrentMetadataPendingResult(string? infoHashV1, string? infoHashV2, string? hash)
        {
            InfoHashV1 = infoHashV1;
            InfoHashV2 = infoHashV2;
            Hash = hash;
        }

        /// <summary>
        /// Gets the v1 info hash when available.
        /// </summary>
        [JsonPropertyName("infohash_v1")]
        public string? InfoHashV1 { get; }

        /// <summary>
        /// Gets the v2 info hash when available.
        /// </summary>
        [JsonPropertyName("infohash_v2")]
        public string? InfoHashV2 { get; }

        /// <summary>
        /// Gets the qBittorrent torrent hash when available.
        /// </summary>
        [JsonPropertyName("hash")]
        public string? Hash { get; }
    }
}
