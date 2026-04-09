namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Represents an RSS folder node.
    /// </summary>
    public sealed record RssFolderItem : RssItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RssFolderItem" /> class.
        /// </summary>
        /// <param name="children">The child items keyed by item name.</param>
        public RssFolderItem(IReadOnlyDictionary<string, RssItem> children)
        {
            Children = children;
        }

        /// <summary>
        /// Gets the child items keyed by item name.
        /// </summary>
        public IReadOnlyDictionary<string, RssItem> Children { get; }
    }
}
