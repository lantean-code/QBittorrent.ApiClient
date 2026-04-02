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
        Unknown,

        /// <summary>
        /// The torrent is force-started and downloading.
        /// </summary>
        [JsonStringEnumMemberName("forcedDL")]
        ForcedDownloading,

        /// <summary>
        /// The torrent is downloading.
        /// </summary>
        [JsonStringEnumMemberName("downloading")]
        Downloading,

        /// <summary>
        /// The torrent is force-started and downloading metadata.
        /// </summary>
        [JsonStringEnumMemberName("forcedMetaDL")]
        ForcedDownloadingMetadata,

        /// <summary>
        /// The torrent is downloading metadata.
        /// </summary>
        [JsonStringEnumMemberName("metaDL")]
        DownloadingMetadata,

        /// <summary>
        /// The torrent is stalled while downloading.
        /// </summary>
        [JsonStringEnumMemberName("stalledDL")]
        StalledDownloading,

        /// <summary>
        /// The torrent is force-started and uploading.
        /// </summary>
        [JsonStringEnumMemberName("forcedUP")]
        ForcedUploading,

        /// <summary>
        /// The torrent is uploading.
        /// </summary>
        [JsonStringEnumMemberName("uploading")]
        Uploading,

        /// <summary>
        /// The torrent is stalled while uploading.
        /// </summary>
        [JsonStringEnumMemberName("stalledUP")]
        StalledUploading,

        /// <summary>
        /// qBittorrent is checking the torrent resume data.
        /// </summary>
        [JsonStringEnumMemberName("checkingResumeData")]
        CheckingResumeData,

        /// <summary>
        /// The torrent is queued for downloading.
        /// </summary>
        [JsonStringEnumMemberName("queuedDL")]
        QueuedDownloading,

        /// <summary>
        /// The torrent is queued for uploading.
        /// </summary>
        [JsonStringEnumMemberName("queuedUP")]
        QueuedUploading,

        /// <summary>
        /// qBittorrent is checking an uploading torrent.
        /// </summary>
        [JsonStringEnumMemberName("checkingUP")]
        CheckingUploading,

        /// <summary>
        /// qBittorrent is checking a downloading torrent.
        /// </summary>
        [JsonStringEnumMemberName("checkingDL")]
        CheckingDownloading,

        /// <summary>
        /// The torrent is stopped before completion.
        /// </summary>
        [JsonStringEnumMemberName("stoppedDL")]
        StoppedDownloading,

        /// <summary>
        /// The torrent is stopped after completion.
        /// </summary>
        [JsonStringEnumMemberName("stoppedUP")]
        StoppedUploading,

        /// <summary>
        /// The torrent content is being moved.
        /// </summary>
        [JsonStringEnumMemberName("moving")]
        Moving,

        /// <summary>
        /// The torrent has missing files.
        /// </summary>
        [JsonStringEnumMemberName("missingFiles")]
        MissingFiles,

        /// <summary>
        /// The torrent is in an error state.
        /// </summary>
        [JsonStringEnumMemberName("error")]
        Error
    }
}
