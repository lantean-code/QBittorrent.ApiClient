using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientTorrentWebSeedsAndLifecycleTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTorrentWebSeedsAndLifecycleTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_Urls_WHEN_AddTorrentWebSeeds_THEN_ShouldPOSTFormWithPipeSeparatedUrls()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/addWebSeeds");
                req.Content!.Headers.ContentType!.MediaType.Should().Be("application/x-www-form-urlencoded");

                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hash=h123&urls=a%7Cb%7Cc");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.AddTorrentWebSeedsAsync("h123", new[] { "a", "b", "c" }, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_AddTorrentWebSeeds_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.AddTorrentWebSeedsAsync("h", new[] { "u" }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_Urls_WHEN_RemoveTorrentWebSeeds_THEN_ShouldPOSTFormWithPipeSeparatedUrls()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/removeWebSeeds");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hash=h1&urls=http%3A%2F%2Fe1%7Chttp%3A%2F%2Fe2");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RemoveTorrentWebSeedsAsync("h1", new[] { "http://e1", "http://e2" }, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_RemoveTorrentWebSeeds_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("conflict")
            });

            var result = await _target.RemoveTorrentWebSeedsAsync("h", new[] { "u" }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "conflict");
        }

        [Fact]
        public async Task GIVEN_EditParams_WHEN_EditTorrentWebSeed_THEN_ShouldPOSTFormWithAllFields()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/editWebSeed");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hash=hx&origUrl=old%2Furl&newUrl=new%2Furl");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.EditTorrentWebSeedAsync("hx", "old/url", "new/url", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_EditTorrentWebSeed_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.EditTorrentWebSeedAsync("h", "o", "n", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_StopTorrents_THEN_ShouldPOSTWithHashValue()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/stop");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=h1");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.StopTorrentsAsync(TorrentSelector.FromHash("h1"), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_AllTrue_WHEN_StopTorrents_THEN_ShouldSendAllLiteral()
        {
            _handler.Responder = async (req, ct) =>
            {
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=all");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.StopTorrentsAsync(TorrentSelector.AllTorrents(), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_StartTorrents_THEN_ShouldPipeSeparate()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/start");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=a%7Cb%7Cc");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.StartTorrentsAsync(TorrentSelector.FromHashes(["a", "b", "c"]), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_DeleteFilesTrue_WHEN_DeleteTorrents_THEN_ShouldIncludeDeleteFlag()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/delete");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=a%7Cb&deleteFiles=true");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DeleteTorrentsAsync(TorrentSelector.FromHashes(["a", "b"]), true, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_AllTrueAndDefaultDeleteFlag_WHEN_DeleteTorrents_THEN_ShouldSendFalseAndAll()
        {
            _handler.Responder = async (req, ct) =>
            {
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=all&deleteFiles=false");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DeleteTorrentsAsync(TorrentSelector.AllTorrents(), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_DeleteTorrents_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.DeleteTorrentsAsync(TorrentSelector.FromHash("h1"), cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_RecheckTorrents_THEN_ShouldPOST()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/recheck");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=h1%7Ch2");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RecheckTorrentsAsync(TorrentSelector.FromHashes(["h1", "h2"]), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_AllTrueAndNoUrls_WHEN_ReannounceTorrents_THEN_ShouldOnlySendHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/reannounce");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=all");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.ReannounceTorrentsAsync(TorrentSelector.AllTorrents(), urls: null, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionAndUrls_WHEN_ReannounceTorrents_THEN_ShouldSendHashesAndUrls()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.1"));

                    case "/torrents/reannounce":
                        return AssertReannounceRequestAsync(req, "hashes=h1%7Ch2&urls=http%3A%2F%2Ft1%7Chttp%3A%2F%2Ft2");

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            await _target.ReannounceTorrentsAsync(TorrentSelector.FromHashes(["h1", "h2"]), urls: new[] { "http://t1", "http://t2" }, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndUrls_WHEN_ReannounceTorrents_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var reannounceRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/torrents/reannounce":
                        reannounceRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.ReannounceTorrentsAsync(TorrentSelector.FromHash("h1"), urls: new[] { "http://t1" }, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.4 does not support tracker-targeted reannounce URLs.");
            reannounceRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailureAndUrls_WHEN_ReannounceTorrents_THEN_ShouldReturnProbeFailure()
        {
            var reannounceRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/reannounce":
                        reannounceRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.ReannounceTorrentsAsync(TorrentSelector.FromHash("h1"), urls: new[] { "http://t1" }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            reannounceRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_ReannounceTorrents_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("nope")
            });

            var result = await _target.ReannounceTorrentsAsync(TorrentSelector.FromHash("h1"), cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "nope");
        }

        private static Task<HttpResponseMessage> AssertReannounceRequestAsync(HttpRequestMessage request, string expectedBody)
        {
            return AssertReannounceRequestAsyncCore(request, expectedBody);
        }

        private static async Task<HttpResponseMessage> AssertReannounceRequestAsyncCore(HttpRequestMessage request, string expectedBody)
        {
            var body = await request.Content!.ReadAsStringAsync(TestContext.Current.CancellationToken);
            body.Should().Be(expectedBody);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string? content)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content ?? string.Empty)
            };
        }
    }
}
