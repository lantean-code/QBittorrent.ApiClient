using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a file within a torrent.
    /// </summary>
    public record FileData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileData" /> class.
        /// </summary>
        [JsonConstructor]
        public FileData(
            int index,
            string name,
            long size,
            float progress,
            Priority priority,
            bool isSeed,
            IReadOnlyList<int> pieceRange,
            float availability)
        {
            Index = index;
            Name = name;
            Size = size;
            Progress = progress;
            Priority = priority;
            IsSeed = isSeed;
            PieceRange = pieceRange ?? [];
            Availability = availability;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        [JsonPropertyName("progress")]
        public float Progress { get; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        [JsonPropertyName("priority")]
        public Priority Priority { get; }

        /// <summary>
        /// Gets a value indicating whether seed.
        /// </summary>
        [JsonPropertyName("is_seed")]
        public bool IsSeed { get; }

        /// <summary>
        /// Gets the piece range.
        /// </summary>
        [JsonPropertyName("piece_range")]
        public IReadOnlyList<int> PieceRange { get; }

        /// <summary>
        /// Gets the availability.
        /// </summary>
        [JsonPropertyName("availability")]
        public float Availability { get; }
    }
}