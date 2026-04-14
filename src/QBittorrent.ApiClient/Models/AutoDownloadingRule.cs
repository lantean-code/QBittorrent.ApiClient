using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents an RSS auto-downloading rule.
    /// </summary>
    public record AutoDownloadingRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDownloadingRule" /> class.
        /// </summary>
        public AutoDownloadingRule()
        {
            AffectedFeeds = [];
            AssignedCategory = "";
            EpisodeFilter = "";
            LastMatch = "";
            MustContain = "";
            MustNotContain = "";
            PreviouslyMatchedEpisodes = [];
            SavePath = "";
            TorrentParams = new();
        }

        /// <summary>
        /// Gets or sets a value indicating whether matched items are added in a paused state.
        /// </summary>
        [JsonPropertyName("addPaused")]
        public bool? AddPaused { get; set; }

        /// <summary>
        /// Gets or sets the affected feeds.
        /// </summary>
        [JsonPropertyName("affectedFeeds")]
        public IReadOnlyList<string> AffectedFeeds { get; set; }

        /// <summary>
        /// Gets or sets the assigned category.
        /// </summary>
        [JsonPropertyName("assignedCategory")]
        public string AssignedCategory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or sets the episode filter.
        /// </summary>
        [JsonPropertyName("episodeFilter")]
        public string EpisodeFilter { get; set; }

        /// <summary>
        /// Gets or sets the ignore days.
        /// </summary>
        [JsonPropertyName("ignoreDays")]
        public int? IgnoreDays { get; set; }

        /// <summary>
        /// Gets or sets the last match.
        /// </summary>
        [JsonPropertyName("lastMatch")]
        public string LastMatch { get; set; }

        /// <summary>
        /// Gets or sets the must contain.
        /// </summary>
        [JsonPropertyName("mustContain")]
        public string MustContain { get; set; }

        /// <summary>
        /// Gets or sets the must not contain.
        /// </summary>
        [JsonPropertyName("mustNotContain")]
        public string MustNotContain { get; set; }

        /// <summary>
        /// Gets or sets the previously matched episodes.
        /// </summary>
        [JsonPropertyName("previouslyMatchedEpisodes")]
        public IReadOnlyList<string> PreviouslyMatchedEpisodes { get; set; }

        /// <summary>
        /// Gets or sets the torrent priority.
        /// </summary>
        [JsonPropertyName("priority")]
        public int? Priority { get; set; }

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        [JsonPropertyName("savePath")]
        public string SavePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether smart filtering is enabled.
        /// </summary>
        [JsonPropertyName("smartFilter")]
        public bool? SmartFilter { get; set; }

        /// <summary>
        /// Gets or sets the torrent content layout.
        /// </summary>
        [JsonPropertyName("torrentContentLayout")]
        public TorrentContentLayout? TorrentContentLayout { get; set; }

        /// <summary>
        /// Gets or sets the torrent params.
        /// </summary>
        [JsonPropertyName("torrentParams")]
        public AutoDownloadingRuleTorrentParams TorrentParams { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether regex is used.
        /// </summary>
        [JsonPropertyName("useRegex")]
        public bool? UseRegex { get; set; }
    }
}
