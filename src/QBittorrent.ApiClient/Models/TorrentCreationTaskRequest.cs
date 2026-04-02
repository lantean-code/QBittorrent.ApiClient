namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a torrent-creation task request.
    /// </summary>
    public record TorrentCreationTaskRequest
    {
        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        public string SourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the torrent file path.
        /// </summary>
        public string? TorrentFilePath { get; set; }

        /// <summary>
        /// Gets or sets the piece size in bytes.
        /// </summary>
        public int? PieceSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the torrent is private.
        /// </summary>
        public bool? Private { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the created torrent starts seeding immediately.
        /// </summary>
        public bool? StartSeeding { get; set; }

        /// <summary>
        /// Gets or sets the torrent comment.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the trackers for the torrent.
        /// </summary>
        public IEnumerable<string>? Trackers { get; set; }

        /// <summary>
        /// Gets or sets the URL seeds.
        /// </summary>
        public IEnumerable<string>? UrlSeeds { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        public TorrentFormat? Format { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether alignment optimization is enabled.
        /// </summary>
        public bool? OptimizeAlignment { get; set; }

        /// <summary>
        /// Gets or sets the padded file size limit in bytes.
        /// </summary>
        public int? PaddedFileSizeLimit { get; set; }
    }
}
