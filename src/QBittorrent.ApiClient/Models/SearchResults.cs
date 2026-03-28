using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents a page of search results.
    /// </summary>
    public record SearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults" /> class.
        /// </summary>
        [JsonConstructor]
        public SearchResults(IReadOnlyList<SearchResult> results, string status, int total)
        {
            Results = results;
            Status = status;
            Total = total;
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        [JsonPropertyName("results")]
        public IReadOnlyList<SearchResult> Results { get; }

        /// <summary>
        /// Gets the tracker status.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; }

        /// <summary>
        /// Gets the total.
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; }
    }
}