using System.Text.Json.Serialization;
using QBittorrent.ApiClient.Converters;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a qBittorrent torrent category.
    /// </summary>
    public record Category
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Category" /> class.
        /// </summary>
        [JsonConstructor]
        public Category(
            string name,
            string? savePath,
            DownloadPathOption? downloadPath)
        {
            Name = name;
            SavePath = savePath;
            DownloadPath = downloadPath;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the save path.
        /// </summary>
        [JsonPropertyName("savePath")]
        public string? SavePath { get; }

        /// <summary>
        /// Gets the download path.
        /// </summary>
        [JsonPropertyName("download_path")]
        [JsonConverter(typeof(DownloadPathOptionJsonConverter))]
        public DownloadPathOption? DownloadPath { get; }
    }
}
