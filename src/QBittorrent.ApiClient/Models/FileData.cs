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
            double progress,
            Priority priority,
            bool? isSeed,
            IReadOnlyList<int> pieceRange,
            double availability)
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
        /// Gets the file size in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; }

        /// <summary>
        /// Gets the file completion fraction from 0.0 to 1.0.
        /// </summary>
        [JsonPropertyName("progress")]
        public double Progress { get; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        [JsonPropertyName("priority")]
        public Priority Priority { get; }

        /// <summary>
        /// Gets a value indicating whether the torrent is seeding or complete, when reported by qBittorrent.
        /// </summary>
        [JsonPropertyName("is_seed")]
        public bool? IsSeed { get; }

        /// <summary>
        /// Gets the piece range.
        /// </summary>
        [JsonPropertyName("piece_range")]
        public IReadOnlyList<int> PieceRange { get; }

        /// <summary>
        /// Gets the availability value reported by qBittorrent in distributed copies.
        /// </summary>
        [JsonPropertyName("availability")]
        public double Availability { get; }
    }
}
