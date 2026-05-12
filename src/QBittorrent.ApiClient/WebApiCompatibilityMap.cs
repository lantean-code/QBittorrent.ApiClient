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

        public static string GetTrackerAllValue(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return webApiVersion >= _trackerAllValueMinimumVersion
                ? "all"
                : "*";
        }
    }
}
