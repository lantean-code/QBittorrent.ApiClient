using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents the main sync payload returned by qBittorrent.
    /// </summary>
    public record MainData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainData" /> class.
        /// </summary>
        [JsonConstructor]
        public MainData(
            int responseId,
            bool fullUpdate,
            IReadOnlyDictionary<string, Torrent>? torrents,
            IReadOnlyList<string>? torrentsRemoved,
            IReadOnlyDictionary<string, Category>? categories,
            IReadOnlyList<string>? categoriesRemoved,
            IReadOnlyList<string>? tags,
            IReadOnlyList<string>? tagsRemoved,
            IReadOnlyDictionary<string, IReadOnlyList<string>>? trackers,
            IReadOnlyList<string>? trackersRemoved,
            ServerState? serverState)
        {
            ResponseId = responseId;
            FullUpdate = fullUpdate;
            Torrents = torrents;
            TorrentsRemoved = torrentsRemoved;
            Categories = categories;
            CategoriesRemoved = categoriesRemoved;
            Tags = tags;
            TagsRemoved = tagsRemoved;
            Trackers = trackers;
            TrackersRemoved = trackersRemoved;
            ServerState = serverState;
        }

        /// <summary>
        /// Gets the response ID.
        /// </summary>
        [JsonPropertyName("rid")]
        public int ResponseId { get; }

        /// <summary>
        /// Gets a value indicating whether the payload contains a full update.
        /// </summary>
        [JsonPropertyName("full_update")]
        public bool FullUpdate { get; }

        /// <summary>
        /// Gets the torrent files keyed by file name.
        /// </summary>
        [JsonPropertyName("torrents")]
        public IReadOnlyDictionary<string, Torrent>? Torrents { get; }

        /// <summary>
        /// Gets the torrents removed.
        /// </summary>
        [JsonPropertyName("torrents_removed")]
        public IReadOnlyList<string>? TorrentsRemoved { get; }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        [JsonPropertyName("categories")]
        public IReadOnlyDictionary<string, Category>? Categories { get; }

        /// <summary>
        /// Gets the categories removed.
        /// </summary>
        [JsonPropertyName("categories_removed")]
        public IReadOnlyList<string>? CategoriesRemoved { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        [JsonPropertyName("tags")]
        public IReadOnlyList<string>? Tags { get; }

        /// <summary>
        /// Gets the tags removed.
        /// </summary>
        [JsonPropertyName("tags_removed")]
        public IReadOnlyList<string>? TagsRemoved { get; }

        /// <summary>
        /// Gets the trackers.
        /// </summary>
        [JsonPropertyName("trackers")]
        public IReadOnlyDictionary<string, IReadOnlyList<string>>? Trackers { get; }

        /// <summary>
        /// Gets the trackers removed.
        /// </summary>
        [JsonPropertyName("trackers_removed")]
        public IReadOnlyList<string>? TrackersRemoved { get; }

        /// <summary>
        /// Gets the server state.
        /// </summary>
        [JsonPropertyName("server_state")]
        public ServerState? ServerState { get; }
    }
}
