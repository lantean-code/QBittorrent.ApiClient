using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a file entry within torrent metadata.
    /// </summary>
    public record TorrentMetadataFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentMetadataFile" /> class.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="length">The file length in bytes.</param>
        [JsonConstructor]
        public TorrentMetadataFile(string? path, long length)
        {
            Path = path ?? throw new JsonException("The torrent metadata file payload did not include a valid path.");
            Length = length;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; }

        /// <summary>
        /// Gets the file length in bytes.
        /// </summary>
        [JsonPropertyName("length")]
        public long Length { get; }
    }
}
