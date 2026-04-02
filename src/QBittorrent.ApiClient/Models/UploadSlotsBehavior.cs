namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies how qBittorrent should allocate upload slots.
    /// </summary>
    public enum UploadSlotsBehavior
    {
        /// <summary>
        /// Uses a fixed number of upload slots.
        /// </summary>
        FixedSlots = 0,

        /// <summary>
        /// Adjusts upload slots based on upload rate.
        /// </summary>
        UploadRateBased = 1
    }
}
