namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Provides special limit values understood by the qBittorrent Web API.
    /// </summary>
    public static class Limits
    {
        /// <summary>
        /// Uses the global qBittorrent share limit for ratio and seeding-time fields.
        /// </summary>
        public const float UseGlobalShareLimit = -2;

        /// <summary>
        /// Removes the qBittorrent share limit for ratio and seeding-time fields.
        /// </summary>
        public const float NoShareLimit = -1;

        /// <summary>
        /// Removes the qBittorrent upload or download rate limit when calling Web API rate-limit setters.
        /// </summary>
        /// <remarks>qBittorrent accepts <c>0</c> through the Web API for upload and download rate setters and normalizes it internally.</remarks>
        public const long NoTransferRateLimit = 0;
    }
}
