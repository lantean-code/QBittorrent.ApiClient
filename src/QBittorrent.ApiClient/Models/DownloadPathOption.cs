namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a category download-path setting.
    /// </summary>
    public record DownloadPathOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadPathOption" /> class.
        /// </summary>
        public DownloadPathOption(bool enabled, string? path)
        {
            Enabled = enabled;
            Path = path;
        }

        /// <summary>
        /// Gets a value indicating whether the item is enabled.
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string? Path { get; }
    }
}
