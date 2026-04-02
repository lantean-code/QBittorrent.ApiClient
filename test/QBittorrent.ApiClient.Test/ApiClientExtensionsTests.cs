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

        private static bool MatchesOptionalHash(TorrentSelector? selector, string hash)
        {
            return selector is not null && MatchesHash(selector, hash);
        }

        [Fact]
        public async Task GIVEN_HashAndNoMatchingTorrent_WHEN_GetTorrent_THEN_ShouldReturnNull()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(null, null, null, null, null, null, null, null, null, null, It.Is<TorrentSelector?>(selector => MatchesOptionalHash(selector, "Hash")), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent>()));

            var result = (await _target.GetTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().BeNull();
        }

        [Fact]
        public async Task GIVEN_HashAndMatchingTorrent_WHEN_GetTorrent_THEN_ShouldReturnFirstTorrent()
        {
            var expectedTorrent = new Torrent(hash: "Hash", name: "Name");

            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(null, null, null, null, null, null, null, null, null, null, It.Is<TorrentSelector?>(selector => MatchesOptionalHash(selector, "Hash")), It.IsAny<CancellationToken>()))
                .Returns(SuccessResult<IReadOnlyList<Torrent>>(new List<Torrent> { expectedTorrent }));

            var result = (await _target.GetTorrentAsync("Hash", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Should().BeSameAs(expectedTorrent);
        }

        [Fact]
        public async Task GIVEN_GetTorrentListFailure_WHEN_GetTorrent_THEN_ShouldReturnFailure()
        {
            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(null, null, null, null, null, null, null, null, null, null, It.Is<TorrentSelector?>(selector => MatchesOptionalHash(selector, "Hash")), It.IsAny<CancellationToken>()))
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
        public async Task GIVEN_UsedAndUnusedCategories_WHEN_RemoveUnusedCategories_THEN_ShouldDeleteOnlyUnusedCategoryNames()
        {
            var torrents = new List<Torrent>
            {
                new Torrent(category: "UsedCategory"),
                new Torrent(category: "UsedCategory"),
                new Torrent(category: null)
            };

            var categories = new Dictionary<string, Category>
            {
                ["UsedCategory"] = new Category("UsedCategory", "SavePath", null),
                ["UnusedCategory"] = new Category("UnusedCategory", "SavePath", null),
                ["NullName"] = new Category(null!, "SavePath", null)
            };

            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
                new Torrent(tags: new List<string> { "UsedTag", "OtherUsedTag", "UsedTag" }),
                new Torrent(tags: null)
            };

            var tags = new List<string>
            {
                "UsedTag",
                "UnusedTag",
                "OtherUsedTag",
                "UnusedTag"
            };

            Mock.Get(_target)
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
                .Setup(apiClient => apiClient.GetTorrentListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.Is<TorrentSelector?>(selector => selector == null), It.IsAny<CancellationToken>()))
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
