namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Provides special limit values understood by the qBittorrent Web API.
    /// </summary>
    public static class Limits
    {
        /// <summary>
        /// Uses the global qBittorrent share ratio limit.
        /// </summary>
        public const float UseGlobalShareRatioLimit = -2;

        /// <summary>
        /// Removes the qBittorrent share ratio limit.
        /// </summary>
        public const float NoShareRatioLimit = -1;

        /// <summary>
        /// Uses the global qBittorrent seeding-time limit for seeding-time fields.
        /// </summary>
        public const int UseGlobalSeedingTimeLimit = -2;

        /// <summary>
        /// Removes the qBittorrent seeding-time limit for seeding-time fields.
        /// </summary>
        public const int NoSeedingTimeLimit = -1;

        /// <summary>
        /// Uses the global qBittorrent inactive seeding-time limit.
        /// </summary>
        public const int UseGlobalInactiveSeedingTimeLimit = -2;

        /// <summary>
        /// Removes the qBittorrent inactive seeding-time limit.
        /// </summary>
        public const int NoInactiveSeedingTimeLimit = -1;

        /// <summary>
        /// Removes the qBittorrent upload or download rate limit when calling Web API rate-limit setters.
        /// </summary>
        /// <remarks>qBittorrent accepts <c>0</c> through the Web API for upload and download rate setters and normalizes it internally.</remarks>
        public const long NoTransferRateLimit = 0;
    }
}
