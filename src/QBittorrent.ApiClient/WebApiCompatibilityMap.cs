using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    internal static class WebApiCompatibilityMap
    {
        private static readonly Version _rssFeedRefreshIntervalMinimumVersion = new(2, 11, 5);
        private static readonly Version _directoryContentMetadataMinimumVersion = new(2, 11, 8);
        private static readonly Version _torrentListIncludeFilesMinimumVersion = new(2, 11, 8);
        private static readonly Version _torrentAddFilePrioritiesMinimumVersion = new(2, 11, 9);
        private static readonly Version _torrentMetadataMinimumVersion = new(2, 11, 9);
        private static readonly Version _trackerBatchOperationsMinimumVersion = new(2, 11, 9);
        private static readonly Version _trackerAllValueMinimumVersion = new(2, 11, 9);
        private static readonly Version _reannounceUrlsMinimumVersion = new(2, 11, 10);
        private static readonly Version _torrentShareLimitActionRequiredMinimumVersion = new(2, 12, 0);
        private static readonly Version _torrentCommentEditingMinimumVersion = new(2, 12, 1);
        private static readonly Version _torrentMetadataArrayResponseMinimumVersion = new(2, 13, 0);
        private static readonly Version _trackerTierEditingMinimumVersion = new(2, 13, 0);
        private static readonly Version _clientDataMinimumVersion = new(2, 13, 1);
        private static readonly Version _torrentAddDownloaderMinimumVersion = new(2, 13, 1);
        private static readonly Version _apiKeyManagementMinimumVersion = new(2, 14, 1);
        private static readonly Version _processInfoMinimumVersion = new(2, 15, 1);
        private static readonly Version _torrentPieceAvailabilityMinimumVersion = new(2, 15, 1);
        private static readonly Version _trackerErrorFiltersMinimumVersion = new(2, 15, 1);
        private static readonly Version _applicationFreeSpaceMinimumVersion = new(2, 16, 2);
        private static readonly Version _searchJobPersistenceMinimumVersion = new(2, 16, 2);
        private static readonly Version _torrentFileBackupsMinimumVersion = new(2, 16, 2);
        private static readonly Version _mailNotificationEncryptionMinimumVersion = new(2, 16, 2);
        private static readonly Version _advancedI2pSettingsMinimumVersion = new(2, 16, 2);
        private static readonly Version _shareLimitsModeMinimumVersion = new(2, 16, 2);
        private static readonly Version _webUiSessionCountLimitMinimumVersion = new(2, 16, 2);
        private static readonly Version _startPausedMinimumVersion = new(2, 16, 2);
        private static readonly Version _shutdownTimeoutMinimumVersion = new(2, 16, 2);
        private static readonly Version _seedingOutgoingConnectionsMinimumVersion = new(2, 16, 2);
        private static readonly Version _multipleConnectionsFromSamePeerIdMinimumVersion = new(2, 16, 2);
        private static readonly Version _maxOutstandingBlockRequestsMinimumVersion = new(2, 16, 2);
        private static readonly Version _webTorrentStunServerMinimumVersion = new(2, 16, 2);
        private static readonly Version _rssRuleExportImportMinimumVersion = new(2, 16, 2);
        private static readonly Version _rssRuleCloningMinimumVersion = new(2, 16, 2);
        private static readonly Version _torrentAddSeedModeMinimumVersion = new(2, 16, 2);
        private static readonly Version _torrentFileDownloadMinimumVersion = new(2, 16, 2);
        private static readonly Version _categoryShareLimitOptionsMinimumVersion = new(2, 16, 2);
        private static readonly Version _speedLimitBatchOperationsMinimumVersion = new(2, 16, 2);
        private static readonly Version _sessionPauseResumeMinimumVersion = new(2, 16, 2);
        private static readonly Version _torrentCreationIgnoreDotfilesMinimumVersion = new(2, 16, 2);

        public static bool TryParseVersion(string? webApiVersion, [NotNullWhen(true)] out Version? parsedApiVersion)
        {
            if (string.IsNullOrWhiteSpace(webApiVersion))
            {
                parsedApiVersion = null;
                return false;
            }

            var normalizedWebApiVersion = webApiVersion.Trim();
            if (!Version.TryParse(normalizedWebApiVersion, out parsedApiVersion))
            {
                parsedApiVersion = null;
                return false;
            }

            return true;
        }

        public static bool SupportsClientData(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _clientDataMinimumVersion;
        }

        public static bool SupportsProcessInfo(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _processInfoMinimumVersion;
        }

        public static bool SupportsApiKeyManagement(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _apiKeyManagementMinimumVersion;
        }

        public static bool SupportsDirectoryContentMetadata(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _directoryContentMetadataMinimumVersion;
        }

        public static bool SupportsRssFeedRefreshInterval(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _rssFeedRefreshIntervalMinimumVersion;
        }

        public static bool SupportsTorrentListIncludeFiles(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentListIncludeFilesMinimumVersion;
        }

        public static bool SupportsTorrentAddDownloader(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentAddDownloaderMinimumVersion;
        }

        public static bool SupportsTorrentAddFilePriorities(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentAddFilePrioritiesMinimumVersion;
        }

        public static bool SupportsTrackerBatchOperations(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _trackerBatchOperationsMinimumVersion;
        }

        public static bool SupportsTrackerTierEditing(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _trackerTierEditingMinimumVersion;
        }

        public static bool SupportsReannounceUrls(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _reannounceUrlsMinimumVersion;
        }

        public static bool SupportsTorrentPieceAvailability(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentPieceAvailabilityMinimumVersion;
        }

        public static bool SupportsTorrentCommentEditing(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentCommentEditingMinimumVersion;
        }

        public static bool SupportsTorrentMetadata(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentMetadataMinimumVersion;
        }

        public static bool SupportsTorrentMetadataArrayResponse(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentMetadataArrayResponseMinimumVersion;
        }

        public static bool RequiresTorrentShareLimitAction(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentShareLimitActionRequiredMinimumVersion;
        }

        public static bool SupportsTrackerErrorFilters(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _trackerErrorFiltersMinimumVersion;
        }

        public static bool SupportsApplicationFreeSpace(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _applicationFreeSpaceMinimumVersion;
        }

        public static bool SupportsSearchJobPersistencePreferences(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _searchJobPersistenceMinimumVersion;
        }

        public static bool SupportsTorrentFileBackupPreferences(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentFileBackupsMinimumVersion;
        }

        public static bool SupportsMailNotificationEncryptionPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _mailNotificationEncryptionMinimumVersion;
        }

        public static bool SupportsAdvancedI2pPreferences(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _advancedI2pSettingsMinimumVersion;
        }

        public static bool SupportsShareLimitsMode(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _shareLimitsModeMinimumVersion;
        }

        public static bool SupportsWebUiSessionCountLimitPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _webUiSessionCountLimitMinimumVersion;
        }

        public static bool SupportsStartPausedPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _startPausedMinimumVersion;
        }

        public static bool SupportsShutdownTimeoutPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _shutdownTimeoutMinimumVersion;
        }

        public static bool SupportsSeedingOutgoingConnectionsPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _seedingOutgoingConnectionsMinimumVersion;
        }

        public static bool SupportsMultipleConnectionsFromSamePeerIdPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _multipleConnectionsFromSamePeerIdMinimumVersion;
        }

        public static bool SupportsMaxOutstandingBlockRequestsPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _maxOutstandingBlockRequestsMinimumVersion;
        }

        public static bool SupportsWebTorrentStunServerPreference(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _webTorrentStunServerMinimumVersion;
        }

        public static bool SupportsRssRuleExportImport(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _rssRuleExportImportMinimumVersion;
        }

        public static bool SupportsRssRuleCloning(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _rssRuleCloningMinimumVersion;
        }

        public static bool UsesTorrentAddSeedMode(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentAddSeedModeMinimumVersion;
        }

        public static bool SupportsTorrentFileDownload(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentFileDownloadMinimumVersion;
        }

        public static bool SupportsCategoryShareLimitOptions(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _categoryShareLimitOptionsMinimumVersion;
        }

        public static bool SupportsSpeedLimitBatchOperations(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _speedLimitBatchOperationsMinimumVersion;
        }

        public static bool SupportsSessionPauseResume(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _sessionPauseResumeMinimumVersion;
        }

        public static bool SupportsTorrentCreationIgnoreDotfiles(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _torrentCreationIgnoreDotfilesMinimumVersion;
        }

        public static string GetTrackerAllValue(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _trackerAllValueMinimumVersion
                ? "all"
                : "*";
        }
    }
}
