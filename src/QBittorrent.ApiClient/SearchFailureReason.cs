namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Identifies the action-specific reason a search request failed.
    /// </summary>
    public enum SearchFailureReason
    {
        /// <summary>
        /// The search job no longer exists.
        /// </summary>
        SearchMissing,

        /// <summary>
        /// The requested result offset is outside the available result range.
        /// </summary>
        OffsetOutOfRange,

        /// <summary>
        /// Search support is unavailable because the Python runtime or plugins are missing.
        /// </summary>
        SearchUnavailable
    }
}
