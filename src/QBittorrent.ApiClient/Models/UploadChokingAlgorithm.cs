namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies which seed choking algorithm qBittorrent should use.
    /// </summary>
    public enum UploadChokingAlgorithm
    {
        /// <summary>
        /// Uses round-robin choking.
        /// </summary>
        RoundRobin = 0,

        /// <summary>
        /// Prefers the fastest uploaders.
        /// </summary>
        FastestUpload = 1,

        /// <summary>
        /// Uses the anti-leech algorithm.
        /// </summary>
        AntiLeech = 2
    }
}
