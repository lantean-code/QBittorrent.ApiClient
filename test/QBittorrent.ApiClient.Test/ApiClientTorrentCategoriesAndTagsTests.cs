using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientTorrentCategoriesAndTagsTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTorrentCategoriesAndTagsTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_OKJson_WHEN_GetAllCategories_THEN_ShouldDeserializeOrReturnUnexpectedResponseOnBadJson()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            var result = (await _target.GetAllCategoriesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();
            result.Should().NotBeNull();
            result.Count.Should().Be(0);

            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("bad")
            });

            var failureResult = await _target.GetAllCategoriesAsync(cancellationToken: TestContext.Current.CancellationToken);

            failureResult.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_CategoryPayload_WHEN_GetAllCategories_THEN_ShouldDeserializeCategoryFields()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                        "Movies":
                        {
                            "name": "Movies",
                            "savePath": "/downloads/movies",
                            "download_path": "/downloads/incomplete"
                        }
                    }
                    """)
            });

            var result = (await _target.GetAllCategoriesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainKey("Movies");
            result["Movies"].Name.Should().Be("Movies");
            result["Movies"].SavePath.Should().Be("/downloads/movies");
            result["Movies"].DownloadPath.Should().NotBeNull();
            result["Movies"].DownloadPath?.Enabled.Should().BeTrue();
            result["Movies"].DownloadPath?.Path.Should().Be("/downloads/incomplete");
        }

        [Fact]
        public async Task GIVEN_CategoryAndPath_WHEN_AddCategory_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/createCategory");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("category=Movies&savePath=%2Fdata");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.AddCategoryAsync("Movies", "/data", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_DownloadPathOption_WHEN_AddCategory_THEN_ShouldIncludeDownloadPathFields()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/createCategory");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("category=Shows&savePath=%2Ftv&downloadPathEnabled=true&downloadPath=%2Ftemp");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.AddCategoryAsync("Shows", "/tv", new DownloadPathOption(true, "/temp"), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_CategoryAndPath_WHEN_EditCategory_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/editCategory");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("category=Shows&savePath=%2Ftv");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.EditCategoryAsync("Shows", "/tv", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_DownloadPathOption_WHEN_EditCategory_THEN_ShouldIncludeDownloadPathFields()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/editCategory");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("category=Music&savePath=%2Fmusic&downloadPathEnabled=false");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.EditCategoryAsync("Music", "/music", new DownloadPathOption(false, null), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_EnabledDownloadPathWithWhitespacePath_WHEN_EditCategory_THEN_ShouldOmitDownloadPathValue()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/editCategory");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("category=Books&savePath=%2Fbooks&downloadPathEnabled=true");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.EditCategoryAsync("Books", "/books", new DownloadPathOption(true, " "), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_EnabledDownloadPathWithValue_WHEN_EditCategory_THEN_ShouldIncludeDownloadPathValue()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/editCategory");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("category=Books&savePath=%2Fbooks&downloadPathEnabled=true&downloadPath=%2Fincomplete");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.EditCategoryAsync("Books", "/books", new DownloadPathOption(true, "/incomplete"), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_EditCategory_THEN_ShouldThrowHttpRequestException()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("edit failed")
            });

            var result = await _target.EditCategoryAsync("Books", "/books", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "edit failed");
        }

        [Fact]
        public async Task GIVEN_Categories_WHEN_RemoveCategories_THEN_ShouldPOSTNewlineSeparated()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/removeCategories");
                var decoded = await req.Content.ReadAsUnescapedStringOrNullAsync(ct);
                decoded.Should().Be("categories=a\nb\nc");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RemoveCategoriesAsync(categories: ["a", "b", "c"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_TagsAndHashes_WHEN_AddTorrentTags_THEN_ShouldCSVAndEncode()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/addTags");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hashes=h1%7Ch2&tags=one%2Ctwo%2Cthree");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.AddTorrentTagsAsync(TorrentSelector.FromHashes(["h1", "h2"]), ["one", "two", "three"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Tags_WHEN_SetTorrentTags_THEN_ShouldPOSTToSetTags()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setTags");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hashes=all&tags=a%2Cb");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentTagsAsync(TorrentSelector.AllTorrents(), ["a", "b"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_TagsAndAllTrue_WHEN_RemoveTorrentTags_THEN_ShouldCSVAndAll()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/removeTags");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hashes=all&tags=a%2Cb");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RemoveTorrentTagsAsync(TorrentSelector.AllTorrents(), ["a", "b"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_OKJson_WHEN_GetAllTags_THEN_ShouldDeserializeList()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[\"x\",\"y\"]")
            });

            var result = (await _target.GetAllTagsAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Should().Be("x");
            result[1].Should().Be("y");
        }

        [Fact]
        public async Task GIVEN_Tags_WHEN_CreateTags_THEN_ShouldCSV()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/createTags");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("tags=a%2Cb%2Cc");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.CreateTagsAsync(["a", "b", "c"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Tags_WHEN_DeleteTags_THEN_ShouldCSV()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/deleteTags");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("tags=a%2Cb");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DeleteTagsAsync(tags: ["a", "b"], cancellationToken: TestContext.Current.CancellationToken);
        }
    }
}
