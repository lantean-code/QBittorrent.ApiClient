using AwesomeAssertions;
using Moq;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public sealed class ApiClientExtensionsTests
    {
        private readonly IApiClient _target;

        public ApiClientExtensionsTests()
        {
            _target = Mock.Of<IApiClient>();
        }

        private static Task<ApiResult> SuccessResult()
        {
            return Task.FromResult(ApiResult.Success());
        }

        private static Task<ApiResult<T>> SuccessResult<T>(T value)
        {
            return Task.FromResult(ApiResult<T>.Success(value));
        }

        private static bool MatchesHash(TorrentSelector selector, string hash)
        {
            return selector.All is false
                && selector.Hashes is not null
                && selector.Hashes.SequenceEqual([hash]);
        }

        private static bool MatchesHashes(TorrentSelector selector, params string[] hashes)
        {
            return selector.All is false
                && selector.Hashes is not null
                && selector.Hashes.SequenceEqual(hashes);
        }

        private static bool MatchesAll(TorrentSelector selector)
        {
            return selector.All && selector.Hashes is null;
        }

        private static bool MatchesOptionalHash(TorrentSelector? selector, string hash)
        {
            return selector is not null && MatchesHash(selector, hash);
        }

        [Fact]
        public async Task GIVEN_NullApiClient_WHEN_StopTorrentAsync_THEN_ShouldThrowArgumentNullException()
        {
            IApiClient? apiClient = null;

            Func<Task> action = () => apiClient!.StopTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken);

            (await action.Should().ThrowAsync<ArgumentNullException>()).Which.ParamName.Should().Be("apiClient");
        }

        [Fact]
        public async Task GIVEN_NullHash_WHEN_StopTorrentAsync_THEN_ShouldThrowArgumentNullException()
        {
            Func<Task> action = () => _target.StopTorrentAsync(null!, cancellationToken: TestContext.Current.CancellationToken);

            (await action.Should().ThrowAsync<ArgumentNullException>()).Which.ParamName.Should().Be("hash");
        }

        [Fact]
        public async Task GIVEN_NullHashes_WHEN_StopTorrentsAsync_THEN_ShouldThrowArgumentNullException()
        {
            Func<Task> action = () => ApiClientExtensions.StopTorrentsAsync(_target, null!, cancellationToken: TestContext.Current.CancellationToken);

            (await action.Should().ThrowAsync<ArgumentNullException>()).Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_HashesContainingNull_WHEN_StopTorrentsAsync_THEN_ShouldThrowArgumentException()
        {
            Func<Task> action = () => ApiClientExtensions.StopTorrentsAsync(_target, new string?[] { "Hash", null! }!, cancellationToken: TestContext.Current.CancellationToken);

            (await action.Should().ThrowAsync<ArgumentException>()).Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_StopTorrent_THEN_ShouldCallStopTorrentsWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.StopTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.StopTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.StopTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_StopTorrents_THEN_ShouldCallStopTorrentsWithArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.StopTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.StopTorrentsAsync(new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.StopTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_NoHashes_WHEN_StopAllTorrents_THEN_ShouldCallStopTorrentsWithAll()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.StopTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesAll(selector)), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.StopAllTorrentsAsync(cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.StopTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesAll(selector)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_StartTorrent_THEN_ShouldCallStartTorrentsWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.StartTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.StartTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.StartTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_StartTorrents_THEN_ShouldCallStartTorrentsWithArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.StartTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.StartTorrentsAsync(new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.StartTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_NoHashes_WHEN_StartAllTorrents_THEN_ShouldCallStartTorrentsWithAll()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.StartTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesAll(selector)), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.StartAllTorrentsAsync(cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.StartTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesAll(selector)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_HashAndDeleteFiles_WHEN_DeleteTorrent_THEN_ShouldCallDeleteTorrentsWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.DeleteTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), true, It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.DeleteTorrentAsync("Hash", true, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.DeleteTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_HashesAndDeleteFiles_WHEN_DeleteTorrents_THEN_ShouldCallDeleteTorrentsWithArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.DeleteTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), false, It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.DeleteTorrentsAsync(new[] { "Hash1", "Hash2" }, false, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.DeleteTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_DeleteFiles_WHEN_DeleteAllTorrents_THEN_ShouldCallDeleteTorrentsWithAll()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.DeleteTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesAll(selector)), true, It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.DeleteAllTorrentsAsync(true, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.DeleteTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesAll(selector)), true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_HashAndNoMatchingTorrent_WHEN_GetTorrent_THEN_ShouldReturnNull()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(null, null, null, null, null, null, null, null, null, null, It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => MatchesOptionalHash(selector, "Hash"))))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent>()));

            var result = (await _target.GetTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().BeNull();
        }

        [Fact]
        public async Task GIVEN_HashAndMatchingTorrent_WHEN_GetTorrent_THEN_ShouldReturnFirstTorrent()
        {
            var expectedTorrent = new Torrent
            {
                Hash = "Hash",
                Name = "Name"
            };

            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(null, null, null, null, null, null, null, null, null, null, It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => MatchesOptionalHash(selector, "Hash"))))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent> { expectedTorrent }));

            var result = (await _target.GetTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Should().BeSameAs(expectedTorrent);
        }

        [Fact]
        public async Task GIVEN_GetTorrentListFailure_WHEN_GetTorrent_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(null, null, null, null, null, null, null, null, null, null, It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => MatchesOptionalHash(selector, "Hash"))))
                .Returns(Task.FromResult(ApiResult<IReadOnlyList<Torrent>>.FailureResult(new ApiFailure
                {
                    Kind = ApiFailureKind.ServerError,
                    Operation = "GetTorrentListAsync",
                    UserMessage = "failed",
                })));

            var result = await _target.GetTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "failed");
        }

        [Fact]
        public async Task GIVEN_CategoryAndHash_WHEN_SetTorrentCategory_THEN_ShouldCallSetTorrentCategoryWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), "Category", It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.SetTorrentCategoryAsync("Category", "Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), "Category", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_CategoryAndHashes_WHEN_SetTorrentCategory_THEN_ShouldCallSetTorrentCategoryWithArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), "Category", It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.SetTorrentCategoryAsync("Category", new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), "Category", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_RemoveTorrentCategory_THEN_ShouldCallSetTorrentCategoryWithEmptyCategory()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), string.Empty, It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.RemoveTorrentCategoryAsync("Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), string.Empty, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_RemoveTorrentCategory_THEN_ShouldCallSetTorrentCategoryWithEmptyCategoryAndArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), string.Empty, It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.RemoveTorrentCategoryAsync(new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.SetTorrentCategoryAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), string.Empty, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagsAndHash_WHEN_RemoveTorrentTags_THEN_ShouldCallRemoveTorrentTagsWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.RemoveTorrentTagsAsync(new[] { "Tag1", "Tag2" }, "Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagsAndHashes_WHEN_RemoveTorrentTags_THEN_ShouldCallRemoveTorrentTagsWithArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.RemoveTorrentTagsAsync(new[] { "Tag1", "Tag2" }, new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagAndHash_WHEN_RemoveTorrentTag_THEN_ShouldCallRemoveTorrentTagsWithSingleTag()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.RemoveTorrentTagAsync("Tag", "Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagAndHashes_WHEN_RemoveTorrentTag_THEN_ShouldCallRemoveTorrentTagsWithSingleTagAndArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.RemoveTorrentTagAsync("Tag", new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.RemoveTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagsAndHash_WHEN_AddTorrentTags_THEN_ShouldCallAddTorrentTagsWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.AddTorrentTagsAsync(new[] { "Tag1", "Tag2" }, "Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagsAndHashes_WHEN_AddTorrentTags_THEN_ShouldCallAddTorrentTagsWithArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.AddTorrentTagsAsync(new[] { "Tag1", "Tag2" }, new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag1", "Tag2" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagAndHash_WHEN_AddTorrentTag_THEN_ShouldCallAddTorrentTagsWithSingleTag()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.AddTorrentTagAsync("Tag", "Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TagAndHashes_WHEN_AddTorrentTag_THEN_ShouldCallAddTorrentTagsWithSingleTagAndArray()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.AddTorrentTagAsync("Tag", new[] { "Hash1", "Hash2" }, cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.AddTorrentTagsAsync(It.Is<TorrentSelector>(selector => MatchesHashes(selector, "Hash1", "Hash2")), It.Is<IEnumerable<string>>(tags => tags.SequenceEqual(new[] { "Tag" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_RecheckTorrent_THEN_ShouldCallRecheckTorrentsWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.RecheckTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.RecheckTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.RecheckTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_ReannounceTorrent_THEN_ShouldCallReannounceTorrentsWithHash()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.ReannounceTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), null, It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            await _target.ReannounceTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken);

            Mock.Get(_target).Verify(apiClient => apiClient.ReannounceTorrentsAsync(It.Is<TorrentSelector>(selector => MatchesHash(selector, "Hash")), null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_UsedAndUnusedCategories_WHEN_RemoveUnusedCategories_THEN_ShouldDeleteOnlyUnusedCategoryNames()
        {
            var torrents = new List<Torrent>
            {
                new Torrent { Category = "UsedCategory" },
                new Torrent { Category = "UsedCategory" },
                new Torrent { Category = null }
            };

            var categories = new Dictionary<string, Category>
            {
                ["UsedCategory"] = new Category("UsedCategory", "SavePath", null),
                ["UnusedCategory"] = new Category("UnusedCategory", "SavePath", null),
                ["NullName"] = new Category(null!, "SavePath", null)
            };

            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(torrents));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .Returns(SuccessResult<IReadOnlyDictionary<string, Category>>(categories));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.RemoveCategoriesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            var result = (await _target.RemoveUnusedCategoriesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Equal("UnusedCategory");
            Mock.Get(_target).Verify(apiClient => apiClient.RemoveCategoriesAsync(It.Is<IEnumerable<string>>(categoryNames => categoryNames.SequenceEqual(new[] { "UnusedCategory" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TorrentListFailure_WHEN_RemoveUnusedCategories_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(Task.FromResult(ApiResult<IReadOnlyList<Torrent>>.FailureResult(new ApiFailure
                {
                    Kind = ApiFailureKind.ServerError,
                    Operation = "GetTorrentListAsync",
                    UserMessage = "torrent failure",
                })));

            var result = await _target.RemoveUnusedCategoriesAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "torrent failure");
        }

        [Fact]
        public async Task GIVEN_CategoriesFailure_WHEN_RemoveUnusedCategories_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent>()));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(ApiResult<IReadOnlyDictionary<string, Category>>.FailureResult(new ApiFailure
                {
                    Kind = ApiFailureKind.ServerError,
                    Operation = "GetAllCategoriesAsync",
                    UserMessage = "category failure",
                })));

            var result = await _target.RemoveUnusedCategoriesAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "category failure");
        }

        [Fact]
        public async Task GIVEN_RemoveCategoriesFailure_WHEN_RemoveUnusedCategories_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent>()));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .Returns(SuccessResult<IReadOnlyDictionary<string, Category>>(new Dictionary<string, Category>
                {
                    ["UnusedCategory"] = new Category("UnusedCategory", "SavePath", null),
                }));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.RemoveCategoriesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(ApiResult.FailureResult(new ApiFailure
                {
                    Kind = ApiFailureKind.ServerError,
                    Operation = "RemoveCategoriesAsync",
                    UserMessage = "remove failure",
                })));

            var result = await _target.RemoveUnusedCategoriesAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "remove failure");
        }

        [Fact]
        public async Task GIVEN_UsedAndUnusedTags_WHEN_RemoveUnusedTags_THEN_ShouldDeleteOnlyUnusedTags()
        {
            var torrents = new List<Torrent>
            {
                new Torrent { Tags = new List<string> { "UsedTag", "OtherUsedTag", "UsedTag" } },
                new Torrent { Tags = null }
            };

            var tags = new List<string>
            {
                "UsedTag",
                "UnusedTag",
                "OtherUsedTag",
                "UnusedTag"
            };

            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(torrents));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetAllTagsAsync(It.IsAny<CancellationToken>()))
                .Returns(SuccessResult<IReadOnlyList<string>>(tags));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.DeleteTagsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult());

            var result = (await _target.RemoveUnusedTagsAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Equal("UnusedTag");
            Mock.Get(_target).Verify(apiClient => apiClient.DeleteTagsAsync(It.Is<IEnumerable<string>>(tagNames => tagNames.SequenceEqual(new[] { "UnusedTag" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GIVEN_TorrentListFailure_WHEN_RemoveUnusedTags_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(Task.FromResult(ApiResult<IReadOnlyList<Torrent>>.FailureResult(new ApiFailure
                {
                    Kind = ApiFailureKind.ServerError,
                    Operation = "GetTorrentListAsync",
                    UserMessage = "torrent failure",
                })));

            var result = await _target.RemoveUnusedTagsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "torrent failure");
        }

        [Fact]
        public async Task GIVEN_TagsFailure_WHEN_RemoveUnusedTags_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent>()));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetAllTagsAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(ApiResult<IReadOnlyList<string>>.FailureResult(new ApiFailure
                {
                    Kind = ApiFailureKind.ServerError,
                    Operation = "GetAllTagsAsync",
                    UserMessage = "tag failure",
                })));

            var result = await _target.RemoveUnusedTagsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "tag failure");
        }

        [Fact]
        public async Task GIVEN_DeleteTagsFailure_WHEN_RemoveUnusedTags_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>(), It.Is<TorrentSelector?>(selector => selector == null)))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent>()));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetAllTagsAsync(It.IsAny<CancellationToken>()))
                .Returns(SuccessResult<IReadOnlyList<string>>(new List<string> { "UnusedTag" }));
            Mock.Get(_target)
                .Setup(apiClient => apiClient.DeleteTagsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(ApiResult.FailureResult(new ApiFailure
                {
                    Kind = ApiFailureKind.ServerError,
                    Operation = "DeleteTagsAsync",
                    UserMessage = "delete failure",
                })));

            var result = await _target.RemoveUnusedTagsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, userMessage: "delete failure");
        }
    }
}
