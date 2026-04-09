using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents an RSS feed node.
    /// </summary>
    public sealed record RssFeedItem : RssItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RssFeedItem" /> class.
        /// </summary>
        /// <param name="articles">The articles returned for the feed.</param>
        /// <param name="hasError">Whether the feed currently has an error.</param>
        /// <param name="isLoading">Whether the feed is currently loading.</param>
        /// <param name="lastBuildDate">The last build date reported by the feed.</param>
        /// <param name="refreshInterval">The refresh interval in seconds.</param>
        /// <param name="title">The feed title.</param>
        /// <param name="uid">The feed unique identifier.</param>
        /// <param name="url">The feed URL.</param>
        public RssFeedItem(
            IReadOnlyList<RssArticle>? articles,
            bool? hasError,
            bool? isLoading,
            string? lastBuildDate,
            long? refreshInterval,
            string? title,
            string uid,
            string url)
        {
            Articles = articles;
            HasError = hasError;
            IsLoading = isLoading;
            LastBuildDate = lastBuildDate;
            RefreshInterval = refreshInterval;
            Title = title;
            Uid = uid;
            Url = url;
        }

        /// <summary>
        /// Gets the articles.
        /// </summary>
        [JsonPropertyName("articles")]
        public IReadOnlyList<RssArticle>? Articles { get; }

        /// <summary>
        /// Gets a value indicating whether the feed has an error.
        /// </summary>
        [JsonPropertyName("hasError")]
        public bool? HasError { get; }

        /// <summary>
        /// Gets a value indicating whether the feed is currently loading.
        /// </summary>
        [JsonPropertyName("isLoading")]
        public bool? IsLoading { get; }

        /// <summary>
        /// Gets the feed last build date.
        /// </summary>
        [JsonPropertyName("lastBuildDate")]
        public string? LastBuildDate { get; }

        /// <summary>
        /// Gets the feed refresh interval in seconds.
        /// </summary>
        [JsonPropertyName("refreshInterval")]
        public long? RefreshInterval { get; }

        /// <summary>
        /// Gets the feed title.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; }

        /// <summary>
        /// Gets the feed unique identifier.
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; }

        /// <summary>
        /// Gets the feed URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }
    }
}
