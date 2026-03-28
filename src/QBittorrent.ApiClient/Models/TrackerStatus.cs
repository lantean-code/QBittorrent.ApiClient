namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies tracker status values reported by qBittorrent.
    /// </summary>
    public enum TrackerStatus
    {
        /// <summary>
        /// Indicates that the tracker is disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Indicates that qBittorrent has not contacted the tracker yet.
        /// </summary>
        Uncontacted = 1,

        /// <summary>
        /// Indicates that the tracker is responding normally.
        /// </summary>
        Working = 2,

        /// <summary>
        /// Indicates that qBittorrent is currently updating the tracker.
        /// </summary>
        Updating = 3,

        /// <summary>
        /// Indicates that the tracker is not working.
        /// </summary>
        NotWorking = 4,

        /// <summary>
        /// Indicates that the tracker reported an error.
        /// </summary>
        Error = 5,

        /// <summary>
        /// Indicates that the tracker could not be reached.
        /// </summary>
        Unreachable = 6
    }
}
