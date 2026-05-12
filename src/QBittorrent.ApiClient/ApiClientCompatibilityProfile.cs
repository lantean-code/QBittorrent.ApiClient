using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    internal sealed class ApiClientCompatibilityProfile
    {
        public Version WebApiVersion { get; }

        public bool SupportsClientData { get; }

        public bool SupportsProcessInfo { get; }

        public bool SupportsApiKeyManagement { get; }

        public bool SupportsDirectoryContentMetadata { get; }

        public bool SupportsRssFeedRefreshInterval { get; }

        public bool SupportsTorrentListIncludeFiles { get; }

        public bool SupportsTorrentAddDownloader { get; }

        public bool SupportsTorrentAddFilePriorities { get; }

        public bool SupportsTrackerBatchOperations { get; }

        public bool SupportsTrackerTierEditing { get; }

        public bool SupportsReannounceUrls { get; }

        public bool SupportsTorrentPieceAvailability { get; }

        public bool SupportsTorrentCommentEditing { get; }

        public bool SupportsTorrentMetadata { get; }

        public bool SupportsTorrentMetadataArrayResponse { get; }

        public bool RequiresTorrentShareLimitAction { get; }

        public bool SupportsTrackerErrorFilters { get; }

        public string TrackerAllValue { get; }

        internal ApiClientCompatibilityProfile(Version webApiVersion)
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
            TrackerAllValue = WebApiCompatibilityMap.GetTrackerAllValue(webApiVersion);
        }

        public static bool TryCreate(string? webApiVersion, [NotNullWhen(true)] out ApiClientCompatibilityProfile? profile)
        {
            if (!WebApiCompatibilityMap.TryParseVersion(webApiVersion, out var parsedApiVersion))
            {
                profile = null;
                return false;
            }

            profile = new ApiClientCompatibilityProfile(parsedApiVersion);
            return true;
        }
    }
}
