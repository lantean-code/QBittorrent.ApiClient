using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents an article returned by an RSS feed.
    /// </summary>
    public class RssArticle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RssArticle" /> class.
        /// </summary>
        [JsonConstructor]
        public RssArticle(
            string? category,
            string? comments,
            string? date,
            string? description,
            string? id,
            string? link,
            string? thumbnail,
            string? title,
            string? torrentURL,
            bool isRead)
        {
            Category = category;
            Comments = comments;
            Date = date;
            Description = description;
            Id = id;
            Link = link;
            Thumbnail = thumbnail;
            Title = title;
            TorrentURL = torrentURL;
            IsRead = isRead;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; }

        /// <summary>
        /// Gets the comments.
        /// </summary>
        [JsonPropertyName("comments")]
        public string? Comments { get; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        [JsonPropertyName("date")]
        public string? Date { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; }

        /// <summary>
        /// Gets the link.
        /// </summary>
        [JsonPropertyName("link")]
        public string? Link { get; }

        /// <summary>
        /// Gets the thumbnail.
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; }

        /// <summary>
        /// Gets the torrent url.
        /// </summary>
        [JsonPropertyName("torrentURL")]
        public string? TorrentURL { get; }

        /// <summary>
        /// Gets a value indicating whether read.
        /// </summary>
        [JsonPropertyName("isRead")]
        public bool IsRead { get; }
    }
}