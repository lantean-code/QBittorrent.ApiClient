using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public sealed class ApiClientCompatibilityProfileCacheTests
    {
        [Fact]
        public void GIVEN_UnknownCacheKey_WHEN_TryGetValue_THEN_ShouldReturnFalse()
        {
            var target = new ApiClientCompatibilityProfileCache();

            var result = target.TryGetValue("http://localhost/api/v2/", out var profile);

            result.Should().BeFalse();
            profile.Should().BeNull();
        }

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
            target.SupportsTorrentShareLimitAction.Should().BeFalse();
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
            target.SupportsTorrentAddFilePriorities.Should().BeTrue();
            target.SupportsTorrentMetadata.Should().BeFalse();
            target.SupportsTrackerBatchOperations.Should().BeFalse();
            target.TrackerAllValue.Should().Be("*");
        }

        [Fact]
        public void GIVEN_Version2119_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableTorrentMetadataAndTrackerBatchFeatures()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 11, 9));

            target.SupportsTorrentMetadata.Should().BeTrue();
            target.SupportsTrackerBatchOperations.Should().BeTrue();
            target.SupportsReannounceUrls.Should().BeFalse();
            target.SupportsTorrentShareLimitAction.Should().BeFalse();
            target.TrackerAllValue.Should().Be("all");
        }

        [Fact]
        public void GIVEN_Version21110_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableTargetedReannounce()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 11, 10));

            target.SupportsTrackerBatchOperations.Should().BeTrue();
            target.SupportsReannounceUrls.Should().BeTrue();
            target.SupportsTorrentShareLimitAction.Should().BeFalse();
            target.TrackerAllValue.Should().Be("all");
        }

        [Fact]
        public void GIVEN_Version2120_WHEN_CreatingCompatibilityProfile_THEN_ShouldSupportShareLimitAction()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 12, 0));

            target.SupportsTorrentShareLimitAction.Should().BeTrue();
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
        public void GIVEN_Version2130_WHEN_CreatingCompatibilityProfile_THEN_ShouldEnableTrackerTierEditing()
        {
            var target = new ApiClientCompatibilityProfile(new Version(2, 13, 0));

            target.SupportsTrackerTierEditing.Should().BeTrue();
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
            target.SupportsTorrentShareLimitAction.Should().BeTrue();
            target.TrackerAllValue.Should().Be("all");
        }

        [Fact]
        public async Task GIVEN_CacheMiss_WHEN_GetOrAddAsync_THEN_ShouldStoreValueForKey()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var expectedProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));

            var result = await target.GetOrAddAsync(
                "http://localhost/api/v2/",
                _ => Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.Success(expectedProfile)),
                TestContext.Current.CancellationToken);

            result.GetValueOrThrow().Should().BeSameAs(expectedProfile);
            target.TryGetValue("http://localhost/api/v2/", out var cachedProfile).Should().BeTrue();
            cachedProfile.Should().BeSameAs(expectedProfile);
        }

        [Fact]
        public async Task GIVEN_CacheHit_WHEN_GetOrAddAsync_THEN_ShouldNotInvokeFactory()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var cachedProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));
            var factoryCallCount = 0;

            await target.TryHydrateAsync("http://localhost/api/v2/", cachedProfile, TestContext.Current.CancellationToken);

            var result = await target.GetOrAddAsync(
                "http://localhost/api/v2/",
                _ =>
                {
                    factoryCallCount++;
                    return Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.Success(new ApiClientCompatibilityProfile(new Version(2, 15, 2))));
                },
                TestContext.Current.CancellationToken);

            result.GetValueOrThrow().Should().BeSameAs(cachedProfile);
            factoryCallCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_FactoryFailure_WHEN_GetOrAddAsync_THEN_ShouldNotCacheFailure()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var factoryCallCount = 0;

            var failureResult = await target.GetOrAddAsync(
                "http://localhost/api/v2/",
                _ =>
                {
                    factoryCallCount++;
                    return Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.FailureResult(new ApiFailure
                    {
                        Kind = ApiFailureKind.ServerError,
                        Operation = "GetOrAddAsync",
                        UserMessage = "failed",
                    }));
                },
                TestContext.Current.CancellationToken);

            failureResult.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "failed");
            target.TryGetValue("http://localhost/api/v2/", out var cachedProfile).Should().BeFalse();
            cachedProfile.Should().BeNull();

            var successProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));
            var successResult = await target.GetOrAddAsync(
                "http://localhost/api/v2/",
                _ =>
                {
                    factoryCallCount++;
                    return Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.Success(successProfile));
                },
                TestContext.Current.CancellationToken);

            successResult.GetValueOrThrow().Should().BeSameAs(successProfile);
            factoryCallCount.Should().Be(2);
        }

        [Fact]
        public async Task GIVEN_DifferentCacheKeys_WHEN_GetOrAddAsync_THEN_ShouldCacheProfilesSeparately()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var firstProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));
            var secondProfile = new ApiClientCompatibilityProfile(new Version(2, 15, 2));

            (await target.GetOrAddAsync(
                "http://localhost-a/api/v2/",
                _ => Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.Success(firstProfile)),
                TestContext.Current.CancellationToken)).ShouldSucceed();

            (await target.GetOrAddAsync(
                "http://localhost-b/api/v2/",
                _ => Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.Success(secondProfile)),
                TestContext.Current.CancellationToken)).ShouldSucceed();

            target.TryGetValue("http://localhost-a/api/v2/", out var cachedFirstProfile).Should().BeTrue();
            target.TryGetValue("http://localhost-b/api/v2/", out var cachedSecondProfile).Should().BeTrue();
            cachedFirstProfile!.WebApiVersion.Should().Be(new Version(2, 13, 1));
            cachedSecondProfile!.WebApiVersion.Should().Be(new Version(2, 15, 2));
        }

        [Fact]
        public async Task GIVEN_ExistingProfileForDifferentKey_WHEN_RefreshAsync_THEN_ShouldReplaceOnlyMatchingKey()
        {
            var target = new ApiClientCompatibilityProfileCache();

            await target.TryHydrateAsync("http://localhost-a/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 13, 1)), TestContext.Current.CancellationToken);
            await target.TryHydrateAsync("http://localhost-b/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 14, 0)), TestContext.Current.CancellationToken);

            var result = await target.RefreshAsync(
                "http://localhost-a/api/v2/",
                _ => Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.Success(new ApiClientCompatibilityProfile(new Version(2, 15, 2)))),
                TestContext.Current.CancellationToken);

            result.ShouldSucceed();
            target.TryGetValue("http://localhost-a/api/v2/", out var refreshedProfile).Should().BeTrue();
            target.TryGetValue("http://localhost-b/api/v2/", out var untouchedProfile).Should().BeTrue();
            refreshedProfile!.WebApiVersion.Should().Be(new Version(2, 15, 2));
            untouchedProfile!.WebApiVersion.Should().Be(new Version(2, 14, 0));
        }

        [Fact]
        public async Task GIVEN_ExistingProfileForKey_WHEN_TryHydrateAsync_THEN_ShouldNotOverwriteCachedValue()
        {
            var target = new ApiClientCompatibilityProfileCache();

            await target.TryHydrateAsync("http://localhost/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 13, 1)), TestContext.Current.CancellationToken);
            await target.TryHydrateAsync("http://localhost/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 15, 2)), TestContext.Current.CancellationToken);

            target.TryGetValue("http://localhost/api/v2/", out var cachedProfile).Should().BeTrue();
            cachedProfile!.WebApiVersion.Should().Be(new Version(2, 13, 1));
        }

        [Fact]
        public async Task GIVEN_FactoryInProgressForDifferentKey_WHEN_GetOrAddAsync_THEN_ShouldNotBlockOtherKey()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var firstFactoryStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var releaseFirstFactory = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var secondFactoryStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var firstTask = target.GetOrAddAsync(
                "http://localhost-a/api/v2/",
                async _ =>
                {
                    firstFactoryStarted.SetResult(true);
                    await releaseFirstFactory.Task.WaitAsync(TestContext.Current.CancellationToken);
                    return ApiResult<ApiClientCompatibilityProfile>.Success(new ApiClientCompatibilityProfile(new Version(2, 13, 1)));
                },
                TestContext.Current.CancellationToken);

            await firstFactoryStarted.Task.WaitAsync(TestContext.Current.CancellationToken);

            var secondTask = target.GetOrAddAsync(
                "http://localhost-b/api/v2/",
                _ =>
                {
                    secondFactoryStarted.SetResult(true);
                    return Task.FromResult(ApiResult<ApiClientCompatibilityProfile>.Success(new ApiClientCompatibilityProfile(new Version(2, 15, 2))));
                },
                TestContext.Current.CancellationToken);

            await secondFactoryStarted.Task.WaitAsync(TestContext.Current.CancellationToken);

            var secondResult = await secondTask.WaitAsync(TestContext.Current.CancellationToken);

            releaseFirstFactory.SetResult(true);

            secondResult.ShouldSucceed();
            (await firstTask).ShouldSucceed();
        }
    }
}
