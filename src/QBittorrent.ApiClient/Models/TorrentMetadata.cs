using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents torrent metadata returned by qBittorrent.
    /// </summary>
    public record TorrentMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentMetadata" /> class.
        /// </summary>
        /// <param name="infoHashV1">The v1 info hash when available.</param>
        /// <param name="infoHashV2">The v2 info hash when available.</param>
        /// <param name="hash">The qBittorrent torrent hash when available.</param>
        /// <param name="info">The nested torrent metadata payload.</param>
        /// <param name="trackers">The trackers declared by the torrent metadata.</param>
        /// <param name="webSeeds">The web seeds declared by the torrent metadata.</param>
        /// <param name="createdBy">The torrent creator when available.</param>
        /// <param name="creationDate">The creation date as a Unix timestamp in seconds when available.</param>
        /// <param name="comment">The torrent comment when available.</param>
        [JsonConstructor]
        public TorrentMetadata(
            string? infoHashV1,
            string? infoHashV2,
            string? hash,
            TorrentMetadataInfo info,
            IReadOnlyList<TorrentMetadataTracker>? trackers,
            IReadOnlyList<string>? webSeeds,
            string? createdBy,
            long? creationDate,
            string? comment)
        {
            InfoHashV1 = infoHashV1;
            InfoHashV2 = infoHashV2;
            Hash = hash;
            Info = info ?? throw new JsonException("The torrent metadata payload did not include a valid info object.");
            Trackers = trackers ?? [];
            WebSeeds = webSeeds ?? [];
            CreatedBy = createdBy;
            CreationDate = creationDate;
            Comment = comment;
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

        /// <summary>
        /// Gets the top-level torrent metadata payload.
        /// </summary>
        [JsonPropertyName("info")]
        public TorrentMetadataInfo Info { get; }

        /// <summary>
        /// Gets the trackers declared by the torrent metadata.
        /// </summary>
        [JsonPropertyName("trackers")]
        public IReadOnlyList<TorrentMetadataTracker> Trackers { get; }

        /// <summary>
        /// Gets the web seeds declared by the torrent metadata.
        /// </summary>
        [JsonPropertyName("webseeds")]
        public IReadOnlyList<string> WebSeeds { get; }

        /// <summary>
        /// Gets the torrent creator when available.
        /// </summary>
        [JsonPropertyName("created_by")]
        public string? CreatedBy { get; }

        /// <summary>
        /// Gets the creation date as a Unix timestamp in seconds when available.
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long? CreationDate { get; }

        /// <summary>
        /// Gets the torrent comment when available.
        /// </summary>
        [JsonPropertyName("comment")]
        public string? Comment { get; }
    }
}
