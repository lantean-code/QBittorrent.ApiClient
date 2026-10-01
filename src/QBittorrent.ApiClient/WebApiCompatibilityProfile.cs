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
        /// Gets a value indicating whether querying free space for a path is supported.
        /// </summary>
        public bool SupportsApplicationFreeSpace { get; }

        /// <summary>
        /// Gets a value indicating whether search-job persistence preferences are supported.
        /// </summary>
        public bool SupportsSearchJobPersistencePreferences { get; }

        /// <summary>
        /// Gets a value indicating whether torrent-file backup preferences are supported.
        /// </summary>
        public bool SupportsTorrentFileBackupPreferences { get; }

        /// <summary>
        /// Gets a value indicating whether selecting the mail-notification encryption type is supported.
        /// </summary>
        public bool SupportsMailNotificationEncryptionPreference { get; }

        /// <summary>
        /// Gets a value indicating whether advanced I2P preferences are supported.
        /// </summary>
        public bool SupportsAdvancedI2pPreferences { get; }

        /// <summary>
        /// Gets a value indicating whether selecting how share limits are combined is supported.
        /// </summary>
        public bool SupportsShareLimitsMode { get; }

        /// <summary>
        /// Gets a value indicating whether limiting concurrent Web UI sessions is supported.
        /// </summary>
        public bool SupportsWebUiSessionCountLimitPreference { get; }

        /// <summary>
        /// Gets a value indicating whether the start-paused application preference is supported.
        /// </summary>
        public bool SupportsStartPausedPreference { get; }

        /// <summary>
        /// Gets a value indicating whether configuring the shutdown timeout is supported.
        /// </summary>
        public bool SupportsShutdownTimeoutPreference { get; }

        /// <summary>
        /// Gets a value indicating whether configuring outgoing connections while seeding is supported.
        /// </summary>
        public bool SupportsSeedingOutgoingConnectionsPreference { get; }

        /// <summary>
        /// Gets a value indicating whether multiple connections from the same peer ID can be enabled.
        /// </summary>
        public bool SupportsMultipleConnectionsFromSamePeerIdPreference { get; }

        /// <summary>
        /// Gets a value indicating whether the maximum outstanding block-request count can be configured.
        /// </summary>
        public bool SupportsMaxOutstandingBlockRequestsPreference { get; }

        /// <summary>
        /// Gets a value indicating whether a WebTorrent STUN server can be configured.
        /// </summary>
        public bool SupportsWebTorrentStunServerPreference { get; }

        /// <summary>
        /// Gets a value indicating whether RSS auto-downloading rules can be exported and imported.
        /// </summary>
        public bool SupportsRssRuleExportImport { get; }

        /// <summary>
        /// Gets a value indicating whether RSS auto-downloading rules can be cloned.
        /// </summary>
        public bool SupportsRssRuleCloning { get; }

        /// <summary>
        /// Gets a value indicating whether add-torrent requests use the seed-mode field.
        /// </summary>
        public bool UsesTorrentAddSeedMode { get; }

        /// <summary>
        /// Gets a value indicating whether individual files can be downloaded from torrents.
        /// </summary>
        public bool SupportsTorrentFileDownload { get; }

        /// <summary>
        /// Gets a value indicating whether category share-limit options are supported.
        /// </summary>
        public bool SupportsCategoryShareLimitOptions { get; }

        /// <summary>
        /// Gets a value indicating whether global speed limits can be read and written as a batch.
        /// </summary>
        public bool SupportsSpeedLimitBatchOperations { get; }

        /// <summary>
        /// Gets a value indicating whether the entire transfer session can be paused and resumed.
        /// </summary>
        public bool SupportsSessionPauseResume { get; }

        /// <summary>
        /// Gets a value indicating whether torrent creation can configure dotfile exclusion.
        /// </summary>
        public bool SupportsTorrentCreationIgnoreDotfiles { get; }

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
            SupportsApplicationFreeSpace = WebApiCompatibilityMap.SupportsApplicationFreeSpace(webApiVersion);
            SupportsSearchJobPersistencePreferences = WebApiCompatibilityMap.SupportsSearchJobPersistencePreferences(webApiVersion);
            SupportsTorrentFileBackupPreferences = WebApiCompatibilityMap.SupportsTorrentFileBackupPreferences(webApiVersion);
            SupportsMailNotificationEncryptionPreference = WebApiCompatibilityMap.SupportsMailNotificationEncryptionPreference(webApiVersion);
            SupportsAdvancedI2pPreferences = WebApiCompatibilityMap.SupportsAdvancedI2pPreferences(webApiVersion);
            SupportsShareLimitsMode = WebApiCompatibilityMap.SupportsShareLimitsMode(webApiVersion);
            SupportsWebUiSessionCountLimitPreference = WebApiCompatibilityMap.SupportsWebUiSessionCountLimitPreference(webApiVersion);
            SupportsStartPausedPreference = WebApiCompatibilityMap.SupportsStartPausedPreference(webApiVersion);
            SupportsShutdownTimeoutPreference = WebApiCompatibilityMap.SupportsShutdownTimeoutPreference(webApiVersion);
            SupportsSeedingOutgoingConnectionsPreference = WebApiCompatibilityMap.SupportsSeedingOutgoingConnectionsPreference(webApiVersion);
            SupportsMultipleConnectionsFromSamePeerIdPreference = WebApiCompatibilityMap.SupportsMultipleConnectionsFromSamePeerIdPreference(webApiVersion);
            SupportsMaxOutstandingBlockRequestsPreference = WebApiCompatibilityMap.SupportsMaxOutstandingBlockRequestsPreference(webApiVersion);
            SupportsWebTorrentStunServerPreference = WebApiCompatibilityMap.SupportsWebTorrentStunServerPreference(webApiVersion);
            SupportsRssRuleExportImport = WebApiCompatibilityMap.SupportsRssRuleExportImport(webApiVersion);
            SupportsRssRuleCloning = WebApiCompatibilityMap.SupportsRssRuleCloning(webApiVersion);
            UsesTorrentAddSeedMode = WebApiCompatibilityMap.UsesTorrentAddSeedMode(webApiVersion);
            SupportsTorrentFileDownload = WebApiCompatibilityMap.SupportsTorrentFileDownload(webApiVersion);
            SupportsCategoryShareLimitOptions = WebApiCompatibilityMap.SupportsCategoryShareLimitOptions(webApiVersion);
            SupportsSpeedLimitBatchOperations = WebApiCompatibilityMap.SupportsSpeedLimitBatchOperations(webApiVersion);
            SupportsSessionPauseResume = WebApiCompatibilityMap.SupportsSessionPauseResume(webApiVersion);
            SupportsTorrentCreationIgnoreDotfiles = WebApiCompatibilityMap.SupportsTorrentCreationIgnoreDotfiles(webApiVersion);
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
