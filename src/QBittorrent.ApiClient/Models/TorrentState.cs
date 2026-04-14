using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Models
{
    /// <summary>
    /// Specifies the state reported for a torrent in qBittorrent.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TorrentState>))]
    public enum TorrentState
    {
        /// <summary>
        /// qBittorrent could not determine the torrent state.
        /// </summary>
        [JsonStringEnumMemberName("unknown")]
        Unknown = -1,

        /// <summary>
        /// The torrent is force-started and downloading.
        /// </summary>
        [JsonStringEnumMemberName("forcedDL")]
        ForcedDownloading = 0,

        /// <summary>
        /// The torrent is downloading.
        /// </summary>
        [JsonStringEnumMemberName("downloading")]
        Downloading = 1,

        /// <summary>
        /// The torrent is force-started and downloading metadata.
        /// </summary>
        [JsonStringEnumMemberName("forcedMetaDL")]
        ForcedDownloadingMetadata = 2,

        /// <summary>
        /// The torrent is downloading metadata.
        /// </summary>
        [JsonStringEnumMemberName("metaDL")]
        DownloadingMetadata = 3,

        /// <summary>
        /// The torrent is stalled while downloading.
        /// </summary>
        [JsonStringEnumMemberName("stalledDL")]
        StalledDownloading = 4,

        /// <summary>
        /// The torrent is force-started and uploading.
        /// </summary>
        [JsonStringEnumMemberName("forcedUP")]
        ForcedUploading = 5,

        /// <summary>
        /// The torrent is uploading.
        /// </summary>
        [JsonStringEnumMemberName("uploading")]
        Uploading = 6,

        /// <summary>
        /// The torrent is stalled while uploading.
        /// </summary>
        [JsonStringEnumMemberName("stalledUP")]
        StalledUploading = 7,

        /// <summary>
        /// qBittorrent is checking the torrent resume data.
        /// </summary>
        [JsonStringEnumMemberName("checkingResumeData")]
        CheckingResumeData = 8,

        /// <summary>
        /// The torrent is queued for downloading.
        /// </summary>
        [JsonStringEnumMemberName("queuedDL")]
        QueuedDownloading = 9,

        /// <summary>
        /// The torrent is queued for uploading.
        /// </summary>
        [JsonStringEnumMemberName("queuedUP")]
        QueuedUploading = 10,

        /// <summary>
        /// qBittorrent is checking an uploading torrent.
        /// </summary>
        [JsonStringEnumMemberName("checkingUP")]
        CheckingUploading = 11,

        /// <summary>
        /// qBittorrent is checking a downloading torrent.
        /// </summary>
        [JsonStringEnumMemberName("checkingDL")]
        CheckingDownloading = 12,

        /// <summary>
        /// The torrent is stopped before completion.
        /// </summary>
        [JsonStringEnumMemberName("stoppedDL")]
        StoppedDownloading = 13,

        /// <summary>
        /// The torrent is stopped after completion.
        /// </summary>
        [JsonStringEnumMemberName("stoppedUP")]
        StoppedUploading = 14,

        /// <summary>
        /// The torrent content is being moved.
        /// </summary>
        [JsonStringEnumMemberName("moving")]
        Moving = 15,

        /// <summary>
        /// The torrent has missing files.
        /// </summary>
        [JsonStringEnumMemberName("missingFiles")]
        MissingFiles = 16,

        /// <summary>
        /// The torrent is in an error state.
        /// </summary>
        [JsonStringEnumMemberName("error")]
        Error = 17
    }
}
