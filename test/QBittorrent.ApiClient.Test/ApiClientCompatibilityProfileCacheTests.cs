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
        public async Task GIVEN_CacheMiss_WHEN_GetOrAddAsync_THEN_ShouldStoreValueForKey()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var expectedProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));

            var result = await target.GetOrAddAsync(
                "http://localhost/api/v2/",
                _ => Task.FromResult<ApiClientCompatibilityProfile?>(expectedProfile),
                TestContext.Current.CancellationToken);

            result.Should().BeSameAs(expectedProfile);
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
                    return Task.FromResult<ApiClientCompatibilityProfile?>(new ApiClientCompatibilityProfile(new Version(2, 15, 2)));
                },
                TestContext.Current.CancellationToken);

            result.Should().BeSameAs(cachedProfile);
            factoryCallCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_CacheHydratedWhileFactoryInProgress_WHEN_GetOrAddAsyncCompletes_THEN_ShouldReturnHydratedProfile()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var factoryProfile = new ApiClientCompatibilityProfile(new Version(2, 12, 0));
            var hydratedProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));
            var factoryStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var releaseFactory = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var resultTask = target.GetOrAddAsync(
                "http://localhost/api/v2/",
                async _ =>
                {
                    factoryStarted.SetResult();
                    await releaseFactory.Task.WaitAsync(TestContext.Current.CancellationToken);
                    return factoryProfile;
                },
                TestContext.Current.CancellationToken);

            await factoryStarted.Task.WaitAsync(TestContext.Current.CancellationToken);

            target.TryHydrate("http://localhost/api/v2/", hydratedProfile);
            releaseFactory.SetResult();

            var result = await resultTask.WaitAsync(TestContext.Current.CancellationToken);

            result.Should().BeSameAs(hydratedProfile);
        }

        [Fact]
        public async Task GIVEN_TryHydrateWhileFactoryInProgress_WHEN_FactoryCompletes_THEN_ShouldNotReplaceCachedValue()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var factoryProfile = new ApiClientCompatibilityProfile(new Version(2, 12, 0));
            var hydratedProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));
            var factoryStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var releaseFactory = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var resultTask = target.GetOrAddAsync(
                "http://localhost/api/v2/",
                async _ =>
                {
                    factoryStarted.SetResult();
                    await releaseFactory.Task.WaitAsync(TestContext.Current.CancellationToken);
                    return factoryProfile;
                },
                TestContext.Current.CancellationToken);

            await factoryStarted.Task.WaitAsync(TestContext.Current.CancellationToken);

            target.TryHydrate("http://localhost/api/v2/", hydratedProfile);
            releaseFactory.SetResult();

            await resultTask.WaitAsync(TestContext.Current.CancellationToken);

            target.TryGetValue("http://localhost/api/v2/", out var cachedProfile).Should().BeTrue();
            cachedProfile.Should().BeSameAs(hydratedProfile);
        }

        [Fact]
        public async Task GIVEN_FactoryFailure_WHEN_GetOrAddAsync_THEN_ShouldNotCacheFailure()
        {
            var target = new ApiClientCompatibilityProfileCache();
            var factoryCallCount = 0;

            var profile = await target.GetOrAddAsync(
                "http://localhost/api/v2/",
                _ =>
                {
                    factoryCallCount++;
                    return Task.FromResult<ApiClientCompatibilityProfile?>(null);
                },
                TestContext.Current.CancellationToken);

            profile.Should().BeNull();
            target.TryGetValue("http://localhost/api/v2/", out var cachedProfile).Should().BeFalse();
            cachedProfile.Should().BeNull();

            var successProfile = new ApiClientCompatibilityProfile(new Version(2, 13, 1));
            var successResult = await target.GetOrAddAsync(
                "http://localhost/api/v2/",
                _ =>
                {
                    factoryCallCount++;
                    return Task.FromResult<ApiClientCompatibilityProfile?>(successProfile);
                },
                TestContext.Current.CancellationToken);

            successResult.Should().BeSameAs(successProfile);
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
                _ => Task.FromResult<ApiClientCompatibilityProfile?>(firstProfile),
                TestContext.Current.CancellationToken)).Should().BeEquivalentTo(firstProfile);

            (await target.GetOrAddAsync(
                "http://localhost-b/api/v2/",
                _ => Task.FromResult<ApiClientCompatibilityProfile?>(secondProfile),
                TestContext.Current.CancellationToken)).Should().BeEquivalentTo(secondProfile);

            target.TryGetValue("http://localhost-a/api/v2/", out var cachedFirstProfile).Should().BeTrue();
            target.TryGetValue("http://localhost-b/api/v2/", out var cachedSecondProfile).Should().BeTrue();
            cachedFirstProfile?.WebApiVersion.Should().Be(new Version(2, 13, 1));
            cachedSecondProfile?.WebApiVersion.Should().Be(new Version(2, 15, 2));
        }

        [Fact]
        public async Task GIVEN_ExistingProfileForDifferentKey_WHEN_RefreshAsync_THEN_ShouldReplaceOnlyMatchingKey()
        {
            var target = new ApiClientCompatibilityProfileCache();

            await target.TryHydrateAsync("http://localhost-a/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 13, 1)), TestContext.Current.CancellationToken);
            await target.TryHydrateAsync("http://localhost-b/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 14, 0)), TestContext.Current.CancellationToken);

            var result = await target.RefreshAsync(
                "http://localhost-a/api/v2/",
                _ => Task.FromResult<ApiClientCompatibilityProfile?>(new ApiClientCompatibilityProfile(new Version(2, 15, 2))),
                TestContext.Current.CancellationToken);

            result.Should().BeTrue();
            target.TryGetValue("http://localhost-a/api/v2/", out var refreshedProfile).Should().BeTrue();
            target.TryGetValue("http://localhost-b/api/v2/", out var untouchedProfile).Should().BeTrue();
            refreshedProfile?.WebApiVersion.Should().Be(new Version(2, 15, 2));
            untouchedProfile?.WebApiVersion.Should().Be(new Version(2, 14, 0));
        }

        [Fact]
        public async Task GIVEN_ExistingProfileForKey_WHEN_TryHydrateAsync_THEN_ShouldNotOverwriteCachedValue()
        {
            var target = new ApiClientCompatibilityProfileCache();

            await target.TryHydrateAsync("http://localhost/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 13, 1)), TestContext.Current.CancellationToken);
            await target.TryHydrateAsync("http://localhost/api/v2/", new ApiClientCompatibilityProfile(new Version(2, 15, 2)), TestContext.Current.CancellationToken);

            target.TryGetValue("http://localhost/api/v2/", out var cachedProfile).Should().BeTrue();
            cachedProfile?.WebApiVersion.Should().Be(new Version(2, 13, 1));
        }

        [Fact]
        public async Task GIVEN_FactoryFailure_WHEN_RefreshAsync_THEN_ShouldReturnFalseAndNotCacheFailure()
        {
            var target = new ApiClientCompatibilityProfileCache();

            var result = await target.RefreshAsync(
                "http://localhost/api/v2/",
                _ => Task.FromResult<ApiClientCompatibilityProfile?>(null),
                TestContext.Current.CancellationToken);

            result.Should().BeFalse();
            target.TryGetValue("http://localhost/api/v2/", out var cachedProfile).Should().BeFalse();
            cachedProfile.Should().BeNull();
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
                    return new ApiClientCompatibilityProfile(new Version(2, 13, 1));
                },
                TestContext.Current.CancellationToken);

            await firstFactoryStarted.Task.WaitAsync(TestContext.Current.CancellationToken);

            var secondTask = target.GetOrAddAsync(
                "http://localhost-b/api/v2/",
                _ =>
                {
                    secondFactoryStarted.SetResult(true);
                    return Task.FromResult<ApiClientCompatibilityProfile?>(new ApiClientCompatibilityProfile(new Version(2, 15, 2)));
                },
                TestContext.Current.CancellationToken);

            await secondFactoryStarted.Task.WaitAsync(TestContext.Current.CancellationToken);

            var secondResult = await secondTask.WaitAsync(TestContext.Current.CancellationToken);

            releaseFirstFactory.SetResult(true);

            secondResult.Should().BeEquivalentTo(new ApiClientCompatibilityProfile(new Version(2, 15, 2)));
            (await firstTask).Should().BeEquivalentTo(new ApiClientCompatibilityProfile(new Version(2, 13, 1)));
        }
    }
}
