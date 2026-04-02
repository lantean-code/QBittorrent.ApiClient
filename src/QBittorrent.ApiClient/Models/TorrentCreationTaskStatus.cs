using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the status of a torrent-creation task.
    /// </summary>
    public record TorrentCreationTaskStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentCreationTaskStatus" /> class.
        /// </summary>
        [JsonConstructor]
        public TorrentCreationTaskStatus(
            string taskID,
            string? sourcePath,
            int? pieceSize,
            bool? @private,
            string? timeAdded,
            TorrentFormat? format,
            bool? optimizeAlignment,
            int? paddedFileSizeLimit,
            TorrentCreationTaskStatusKind? status,
            string? comment,
            string? torrentFilePath,
            string? source,
            IReadOnlyList<string>? trackers,
            IReadOnlyList<string>? urlSeeds,
            string? timeStarted,
            string? timeFinished,
            string? errorMessage,
            int? progress)
        {
            TaskId = taskID;
            SourcePath = sourcePath;
            PieceSize = pieceSize;
            Private = @private;
            TimeAdded = timeAdded;
            Format = format;
            OptimizeAlignment = optimizeAlignment;
            PaddedFileSizeLimit = paddedFileSizeLimit;
            Status = status;
            Comment = comment;
            TorrentFilePath = torrentFilePath;
            Source = source;
            Trackers = trackers ?? Array.Empty<string>();
            UrlSeeds = urlSeeds ?? Array.Empty<string>();
            TimeStarted = timeStarted;
            TimeFinished = timeFinished;
            ErrorMessage = errorMessage;
            Progress = progress;
        }

        /// <summary>
        /// Gets the task ID.
        /// </summary>
        [JsonPropertyName("taskID")]
        public string TaskId { get; }

        /// <summary>
        /// Gets the source path.
        /// </summary>
        [JsonPropertyName("sourcePath")]
        public string? SourcePath { get; }

        /// <summary>
        /// Gets the piece size in bytes.
        /// </summary>
        [JsonPropertyName("pieceSize")]
        public int? PieceSize { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent is private.
        /// </summary>
        [JsonPropertyName("private")]
        public bool? Private { get; }

        /// <summary>
        /// Gets the time added.
        /// </summary>
        [JsonPropertyName("timeAdded")]
        public string? TimeAdded { get; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        [JsonPropertyName("format")]
        public TorrentFormat? Format { get; }

        /// <summary>
        /// Gets a value indicating whether alignment optimization is enabled.
        /// </summary>
        [JsonPropertyName("optimizeAlignment")]
        public bool? OptimizeAlignment { get; }

        /// <summary>
        /// Gets the padded file size limit in bytes.
        /// </summary>
        [JsonPropertyName("paddedFileSizeLimit")]
        public int? PaddedFileSizeLimit { get; }

        /// <summary>
        /// Gets the torrent-creation task status.
        /// </summary>
        [JsonPropertyName("status")]
        public TorrentCreationTaskStatusKind? Status { get; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        [JsonPropertyName("comment")]
        public string? Comment { get; }

        /// <summary>
        /// Gets the torrent file path.
        /// </summary>
        [JsonPropertyName("torrentFilePath")]
        public string? TorrentFilePath { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; }

        /// <summary>
        /// Gets the trackers.
        /// </summary>
        [JsonPropertyName("trackers")]
        public IReadOnlyList<string> Trackers { get; }

        /// <summary>
        /// Gets the URL seeds.
        /// </summary>
        [JsonPropertyName("urlSeeds")]
        public IReadOnlyList<string> UrlSeeds { get; }

        /// <summary>
        /// Gets the time started.
        /// </summary>
        [JsonPropertyName("timeStarted")]
        public string? TimeStarted { get; }

        /// <summary>
        /// Gets the time finished.
        /// </summary>
        [JsonPropertyName("timeFinished")]
        public string? TimeFinished { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; }

        /// <summary>
        /// Gets the creation progress as a percentage from 0 to 100.
        /// </summary>
        [JsonPropertyName("progress")]
        public int? Progress { get; }
    }
}
