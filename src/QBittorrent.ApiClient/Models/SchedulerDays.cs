namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies when qBittorrent should apply scheduled alternative rate limits.
    /// </summary>
    public enum SchedulerDays
    {
        /// <summary>
        /// Applies the schedule every day.
        /// </summary>
        EveryDay = 0,

        /// <summary>
        /// Applies the schedule on weekdays.
        /// </summary>
        Weekdays = 1,

        /// <summary>
        /// Applies the schedule on weekends.
        /// </summary>
        Weekends = 2,

        /// <summary>
        /// Applies the schedule on Monday.
        /// </summary>
        Monday = 3,

        /// <summary>
        /// Applies the schedule on Tuesday.
        /// </summary>
        Tuesday = 4,

        /// <summary>
        /// Applies the schedule on Wednesday.
        /// </summary>
        Wednesday = 5,

        /// <summary>
        /// Applies the schedule on Thursday.
        /// </summary>
        Thursday = 6,

        /// <summary>
        /// Applies the schedule on Friday.
        /// </summary>
        Friday = 7,

        /// <summary>
        /// Applies the schedule on Saturday.
        /// </summary>
        Saturday = 8,

        /// <summary>
        /// Applies the schedule on Sunday.
        /// </summary>
        Sunday = 9
    }
}
