using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the status of a search job.
    /// </summary>
    public record SearchStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchStatus" /> class.
        /// </summary>
        [JsonConstructor]
        public SearchStatus(
            int id,
            SearchJobStatus status,
            int total,
            string? pattern = null,
            string? category = null,
            IReadOnlyList<string>? plugins = null)
        {
            Id = id;
            Status = status;
            Total = total;
            Pattern = pattern;
            Category = category;
            Plugins = plugins ?? [];
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; }

        /// <summary>
        /// Gets the search job status.
        /// </summary>
        [JsonPropertyName("status")]
        public SearchJobStatus Status { get; }

        /// <summary>
        /// Gets the total.
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; }

        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        [JsonPropertyName("pattern")]
        public string? Pattern { get; }

        /// <summary>
        /// Gets the search category.
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; }

        /// <summary>
        /// Gets the search plugins.
        /// </summary>
        [JsonPropertyName("plugins")]
        public IReadOnlyList<string> Plugins { get; }
    }
}
