using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public sealed class ApiClientCompatibilityProfileTests
    {
        [Fact]
        public void GIVEN_Version2114_WHEN_CreatingCompatibilityProfile_THEN_ShouldDisableNewerFeatureGates()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 11, 4));

            target.SupportsClientData.Should().BeFalse();
            target.SupportsProcessInfo.Should().BeFalse();
            target.SupportsApiKeyManagement.Should().BeFalse();
            target.SupportsDirectoryContentMetadata.Should().BeFalse();
            target.SupportsRssFeedRefreshInterval.Should().BeFalse();
            target.SupportsTorrentListIncludeFiles.Should().BeFalse();
            target.SupportsTorrentAddDownloader.Should().BeFalse();
            target.SupportsTorrentAddFilePriorities.Should().BeFalse();
            target.SupportsTrackerBatchOperations.Should().BeFalse();
            target.SupportsTrackerTierEditing.Should().BeFalse();
            target.SupportsReannounceUrls.Should().BeFalse();
            target.SupportsTorrentPieceAvailability.Should().BeFalse();
            target.SupportsTorrentCommentEditing.Should().BeFalse();
            target.SupportsTorrentMetadata.Should().BeFalse();
            target.RequiresTorrentShareLimitAction.Should().BeFalse();
            target.SupportsTrackerErrorFilters.Should().BeFalse();
            target.TrackerAllValue.Should().Be("*");
        }

        [Fact]
        public void GIVEN_Version2115_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableRssFeedRefreshIntervalOnly()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 11, 5));

            target.SupportsRssFeedRefreshInterval.Should().BeTrue();
            target.SupportsDirectoryContentMetadata.Should().BeFalse();
            target.SupportsTorrentListIncludeFiles.Should().BeFalse();
            target.SupportsTorrentAddFilePriorities.Should().BeFalse();
            target.SupportsTorrentMetadata.Should().BeFalse();
            target.SupportsTrackerBatchOperations.Should().BeFalse();
            target.TrackerAllValue.Should().Be("*");
        }

        [Fact]
        public void GIVEN_Version2118_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableDirectoryAndTorrentFileFeatures()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 11, 8));

            target.SupportsDirectoryContentMetadata.Should().BeTrue();
            target.SupportsRssFeedRefreshInterval.Should().BeTrue();
            target.SupportsTorrentListIncludeFiles.Should().BeTrue();
            target.SupportsTorrentAddFilePriorities.Should().BeFalse();
            target.SupportsTorrentMetadata.Should().BeFalse();
            target.SupportsTrackerBatchOperations.Should().BeFalse();
            target.TrackerAllValue.Should().Be("*");
        }

        [Fact]
        public void GIVEN_Version2119_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableTorrentMetadataAndTrackerBatchFeatures()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 11, 9));

            target.SupportsTorrentAddFilePriorities.Should().BeTrue();
            target.SupportsTorrentMetadata.Should().BeTrue();
            target.SupportsTorrentMetadataArrayResponse.Should().BeFalse();
            target.SupportsTrackerBatchOperations.Should().BeTrue();
            target.SupportsReannounceUrls.Should().BeFalse();
            target.RequiresTorrentShareLimitAction.Should().BeFalse();
            target.TrackerAllValue.Should().Be("all");
        }

        [Fact]
        public void GIVEN_Version21110_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableTargetedReannounce()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 11, 10));

            target.SupportsTrackerBatchOperations.Should().BeTrue();
            target.SupportsReannounceUrls.Should().BeTrue();
            target.RequiresTorrentShareLimitAction.Should().BeFalse();
            target.TrackerAllValue.Should().Be("all");
        }

        [Fact]
        public void GIVEN_Version2120_WHEN_CreatingCompatibilityProfile_THEN_ShouldRequireShareLimitAction()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 12, 0));

            target.RequiresTorrentShareLimitAction.Should().BeTrue();
            target.SupportsTorrentCommentEditing.Should().BeFalse();
            target.SupportsTrackerTierEditing.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_Version2121_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableTorrentCommentEditing()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 12, 1));

            target.SupportsTorrentCommentEditing.Should().BeTrue();
            target.SupportsTrackerTierEditing.Should().BeFalse();
            target.SupportsClientData.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_Version2130_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableTrackerTierEditingAndTorrentMetadataArrayResponses()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 13, 0));

            target.SupportsTrackerTierEditing.Should().BeTrue();
            target.SupportsTorrentMetadataArrayResponse.Should().BeTrue();
            target.SupportsClientData.Should().BeFalse();
            target.SupportsTorrentAddDownloader.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_Version2131_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableClientDataAndDownloaderFeatures()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 13, 1));

            target.SupportsClientData.Should().BeTrue();
            target.SupportsTorrentAddDownloader.Should().BeTrue();
            target.SupportsApiKeyManagement.Should().BeFalse();
            target.SupportsProcessInfo.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_Version2141_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableApiKeyManagement()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 14, 1));

            target.SupportsClientData.Should().BeTrue();
            target.SupportsApiKeyManagement.Should().BeTrue();
            target.SupportsProcessInfo.Should().BeFalse();
            target.SupportsTorrentPieceAvailability.Should().BeFalse();
            target.SupportsTrackerErrorFilters.Should().BeFalse();
        }

        [Fact]
        public void GIVEN_Version2151_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableLatestFeatures()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 15, 1));

            target.SupportsClientData.Should().BeTrue();
            target.SupportsProcessInfo.Should().BeTrue();
            target.SupportsApiKeyManagement.Should().BeTrue();
            target.SupportsDirectoryContentMetadata.Should().BeTrue();
            target.SupportsRssFeedRefreshInterval.Should().BeTrue();
            target.SupportsTorrentListIncludeFiles.Should().BeTrue();
            target.SupportsTorrentAddDownloader.Should().BeTrue();
            target.SupportsTorrentAddFilePriorities.Should().BeTrue();
            target.SupportsTrackerBatchOperations.Should().BeTrue();
            target.SupportsTrackerTierEditing.Should().BeTrue();
            target.SupportsReannounceUrls.Should().BeTrue();
            target.SupportsTorrentPieceAvailability.Should().BeTrue();
            target.SupportsTorrentCommentEditing.Should().BeTrue();
            target.SupportsTorrentMetadata.Should().BeTrue();
            target.SupportsTorrentMetadataArrayResponse.Should().BeTrue();
            target.RequiresTorrentShareLimitAction.Should().BeTrue();
            target.SupportsTrackerErrorFilters.Should().BeTrue();
            target.TrackerAllValue.Should().Be("all");
        }

        [Fact]
        public void GIVEN_ValidVersionString_WHEN_TryCreateCompatibilityProfile_THEN_ShouldReturnProfile()
        {
            var result = ApiClientCompatibilityProfile.TryCreate("2.13.1", out var profile);

            result.Should().BeTrue();
            profile.Should().NotBeNull();
            profile!.WebApiVersion.Should().Be(new Version(2, 13, 1));
        }

        [Fact]
        public void GIVEN_WhitespacePaddedVersionString_WHEN_TryCreateCompatibilityProfile_THEN_ShouldReturnProfile()
        {
            var result = ApiClientCompatibilityProfile.TryCreate(" 2.13.1 ", out var profile);

            result.Should().BeTrue();
            profile.Should().NotBeNull();
            profile!.WebApiVersion.Should().Be(new Version(2, 13, 1));
        }

        [Fact]
        public void GIVEN_InvalidVersionString_WHEN_TryCreateCompatibilityProfile_THEN_ShouldReturnFalse()
        {
            var result = ApiClientCompatibilityProfile.TryCreate("invalid", out var profile);

            result.Should().BeFalse();
            profile.Should().BeNull();
        }

        [Fact]
        public void GIVEN_NullVersionString_WHEN_TryCreateCompatibilityProfile_THEN_ShouldReturnFalse()
        {
            var result = ApiClientCompatibilityProfile.TryCreate(null, out var profile);

            result.Should().BeFalse();
            profile.Should().BeNull();
        }
    }
}
