namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies qBittorrent log-entry types.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Includes standard log entries.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Includes informational log entries.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Includes warning log entries.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Includes critical log entries.
        /// </summary>
        Critical = 8
    }
}
