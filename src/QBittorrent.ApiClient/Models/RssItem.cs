using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents an RSS feed or folder.
    /// </summary>
    public record RssItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RssItem" /> class.
        /// </summary>
        [JsonConstructor]
        public RssItem(
            IReadOnlyList<RssArticle>? articles,
            bool hasError,
            bool isLoading,
            string? lastBuildDate,
            string? title,
            string uid,
            string url)
        {
            Articles = articles;
            HasError = hasError;
            IsLoading = isLoading;
            LastBuildDate = lastBuildDate;
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
        /// Gets a value indicating whether error.
        /// </summary>
        [JsonPropertyName("hasError")]
        public bool HasError { get; }

        /// <summary>
        /// Gets a value indicating whether loading.
        /// </summary>
        [JsonPropertyName("IsLoading")]
        public bool IsLoading { get; }

        /// <summary>
        /// Gets the last build date.
        /// </summary>
        [JsonPropertyName("lastBuildDate")]
        public string? LastBuildDate { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; }

        /// <summary>
        /// Gets the uid.
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }
    }
}