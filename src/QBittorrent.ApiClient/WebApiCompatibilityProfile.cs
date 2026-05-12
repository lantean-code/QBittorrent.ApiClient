using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Describes qBittorrent Web API compatibility capabilities for a specific Web API version.
    /// </summary>
    public sealed class WebApiCompatibilityProfile
    {
        /// <summary>
        /// Gets the qBittorrent Web API version represented by this compatibility profile.
        /// </summary>
        public Version WebApiVersion { get; }

        /// <summary>
        /// Gets a value indicating whether the qBittorrent client data API is supported.
        /// </summary>
        public bool SupportsClientData { get; }

        /// <summary>
        /// Gets a value indicating whether qBittorrent process information is supported.
        /// </summary>
        public bool SupportsProcessInfo { get; }

        /// <summary>
        /// Gets a value indicating whether qBittorrent Web API key management is supported.
        /// </summary>
        public bool SupportsApiKeyManagement { get; }

        /// <summary>
        /// Gets a value indicating whether directory-content metadata responses are supported.
        /// </summary>
        public bool SupportsDirectoryContentMetadata { get; }

        /// <summary>
        /// Gets a value indicating whether RSS feed refresh intervals are supported.
        /// </summary>
        public bool SupportsRssFeedRefreshInterval { get; }

        /// <summary>
        /// Gets a value indicating whether torrent-list responses can include file data.
        /// </summary>
        public bool SupportsTorrentListIncludeFiles { get; }

        /// <summary>
        /// Gets a value indicating whether selecting a downloader when adding torrents is supported.
        /// </summary>
        public bool SupportsTorrentAddDownloader { get; }

        /// <summary>
        /// Gets a value indicating whether initial file priorities can be specified when adding torrents.
        /// </summary>
        public bool SupportsTorrentAddFilePriorities { get; }

        /// <summary>
        /// Gets a value indicating whether tracker batch operations are supported.
        /// </summary>
        public bool SupportsTrackerBatchOperations { get; }

        /// <summary>
        /// Gets a value indicating whether tracker tier editing is supported.
        /// </summary>
        public bool SupportsTrackerTierEditing { get; }

        /// <summary>
        /// Gets a value indicating whether targeted tracker reannounce URLs are supported.
        /// </summary>
        public bool SupportsReannounceUrls { get; }

        /// <summary>
        /// Gets a value indicating whether torrent piece availability is supported.
        /// </summary>
        public bool SupportsTorrentPieceAvailability { get; }

        /// <summary>
        /// Gets a value indicating whether editing torrent comments is supported.
        /// </summary>
        public bool SupportsTorrentCommentEditing { get; }

        /// <summary>
        /// Gets a value indicating whether torrent metadata endpoints are supported.
        /// </summary>
        public bool SupportsTorrentMetadata { get; }

        /// <summary>
        /// Gets a value indicating whether torrent metadata endpoints can return array responses.
        /// </summary>
        public bool SupportsTorrentMetadataArrayResponse { get; }

        /// <summary>
        /// Gets a value indicating whether torrent share-limit operations require an explicit action parameter.
        /// </summary>
        public bool RequiresTorrentShareLimitAction { get; }

        /// <summary>
        /// Gets a value indicating whether tracker error filters are supported.
        /// </summary>
        public bool SupportsTrackerErrorFilters { get; }

        /// <summary>
        /// Initializes a new compatibility profile for the specified qBittorrent Web API version.
        /// </summary>
        /// <param name="webApiVersion">The qBittorrent Web API version.</param>
        public WebApiCompatibilityProfile(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            WebApiVersion = webApiVersion;
            SupportsClientData = WebApiCompatibilityMap.SupportsClientData(webApiVersion);
            SupportsProcessInfo = WebApiCompatibilityMap.SupportsProcessInfo(webApiVersion);
            SupportsApiKeyManagement = WebApiCompatibilityMap.SupportsApiKeyManagement(webApiVersion);
            SupportsDirectoryContentMetadata = WebApiCompatibilityMap.SupportsDirectoryContentMetadata(webApiVersion);
            SupportsRssFeedRefreshInterval = WebApiCompatibilityMap.SupportsRssFeedRefreshInterval(webApiVersion);
            SupportsTorrentListIncludeFiles = WebApiCompatibilityMap.SupportsTorrentListIncludeFiles(webApiVersion);
            SupportsTorrentAddDownloader = WebApiCompatibilityMap.SupportsTorrentAddDownloader(webApiVersion);
            SupportsTorrentAddFilePriorities = WebApiCompatibilityMap.SupportsTorrentAddFilePriorities(webApiVersion);
            SupportsTrackerBatchOperations = WebApiCompatibilityMap.SupportsTrackerBatchOperations(webApiVersion);
            SupportsTrackerTierEditing = WebApiCompatibilityMap.SupportsTrackerTierEditing(webApiVersion);
            SupportsReannounceUrls = WebApiCompatibilityMap.SupportsReannounceUrls(webApiVersion);
            SupportsTorrentPieceAvailability = WebApiCompatibilityMap.SupportsTorrentPieceAvailability(webApiVersion);
            SupportsTorrentCommentEditing = WebApiCompatibilityMap.SupportsTorrentCommentEditing(webApiVersion);
            SupportsTorrentMetadata = WebApiCompatibilityMap.SupportsTorrentMetadata(webApiVersion);
            SupportsTorrentMetadataArrayResponse = WebApiCompatibilityMap.SupportsTorrentMetadataArrayResponse(webApiVersion);
            RequiresTorrentShareLimitAction = WebApiCompatibilityMap.RequiresTorrentShareLimitAction(webApiVersion);
            SupportsTrackerErrorFilters = WebApiCompatibilityMap.SupportsTrackerErrorFilters(webApiVersion);
        }

        /// <summary>
        /// Attempts to create a compatibility profile from a qBittorrent Web API version string.
        /// </summary>
        /// <param name="webApiVersion">The qBittorrent Web API version string.</param>
        /// <param name="profile">The created compatibility profile when parsing succeeds; otherwise, <see langword="null" />.</param>
        /// <returns><see langword="true" /> when the version string can be parsed into a compatibility profile; otherwise, <see langword="false" />.</returns>
        public static bool TryCreate(string? webApiVersion, [NotNullWhen(true)] out WebApiCompatibilityProfile? profile)
        {
            if (!WebApiCompatibilityMap.TryParseVersion(webApiVersion, out var parsedApiVersion))
            {
                profile = null;
                return false;
            }

            profile = new WebApiCompatibilityProfile(parsedApiVersion);
            return true;
        }
    }
}
