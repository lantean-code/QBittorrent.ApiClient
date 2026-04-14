namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Represents the normalized state of a qBittorrent API operation.
    /// </summary>
    public enum ApiResultStatus
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The operation was accepted but has not completed yet.
        /// </summary>
        Pending,

        /// <summary>
        /// The operation failed.
        /// </summary>
        Failure
    }
}
