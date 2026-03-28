using AwesomeAssertions;
using QBittorrent.ApiClient.Models;
using System.Net;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientTorrentPriorityAndLimitsTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTorrentPriorityAndLimitsTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_IncreaseTorrentPriority_THEN_ShouldPostHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/increasePrio");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("hashes=h1%7Ch2");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.IncreaseTorrentPriorityAsync(all: false, hashes: ["h1", "h2"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_AllTrue_WHEN_DecreaseTorrentPriority_THEN_ShouldPostAll()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/decreasePrio");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("hashes=all");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DecreaseTorrentPriorityAsync(true, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_MaxTorrentPriority_THEN_ShouldPostHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/topPrio");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("hashes=h");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.MaxTorrentPriorityAsync(all: false, hashes: ["h"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_MinTorrentPriority_THEN_ShouldPostHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/bottomPrio");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("hashes=h1%7Ch2%7Ch3");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.MinTorrentPriorityAsync(all: false, hashes: ["h1", "h2", "h3"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_FileIdsAndPriority_WHEN_SetFilePriority_THEN_ShouldPostIdsAndPriorityInt()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/filePrio");
                var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                body.Should().Be("hash=h1&id=1|2|3&priority=7");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetFilePriorityAsync("h1", new[] { 1, 2, 3 }, (Priority)7, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_GetTorrentDownloadLimit_THEN_ShouldReturnDictionary()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"h1\":1000,\"h2\":0}")
            });

            var result = (await _target.GetTorrentDownloadLimitAsync(all: false, hashes: ["h1", "h2"], cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result["h1"].Should().Be(1000);
            result["h2"].Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_BadJson_WHEN_GetTorrentDownloadLimit_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("oops")
            });

            var result = await _target.GetTorrentDownloadLimitAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_LimitAndHashes_WHEN_SetTorrentDownloadLimit_THEN_ShouldPostLimitAndHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/setDownloadLimit");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=h%7Ci&limit=500");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentDownloadLimitAsync(500, all: false, hashes: ["h", "i"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndAction_WHEN_SetTorrentShareLimit_THEN_ShouldOmitActionField()
        {
            var ratio = 1.5f.ToString();
            var seed = 2.25f.ToString();
            var inactive = 0.75f.ToString();

            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.4");

                    case "/torrents/setShareLimits":
                        var form = await req.Content!.ReadAsStringAsync(ct);
                        var parts = form.Split('&').ToDictionary(
                            s => s.Split('=')[0],
                            s => Uri.UnescapeDataString(s.Split('=')[1])
                        );

                        parts["hashes"].Should().Be("h1|h2");
                        parts["ratioLimit"].Should().Be(ratio);
                        parts["seedingTimeLimit"].Should().Be(seed);
                        parts["inactiveSeedingTimeLimit"].Should().Be(inactive);
                        parts.ContainsKey("shareLimitAction").Should().BeFalse();

                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.SetTorrentShareLimitAsync(
                ratioLimit: 1.5f,
                seedingTimeLimit: 2.25f,
                inactiveSeedingTimeLimit: 0.75f,
                shareLimitAction: ShareLimitAction.Remove,
                all: false,
                hashes: new[] { "h1", "h2" }
            , cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionAndAction_WHEN_SetTorrentShareLimit_THEN_ShouldIncludeActionField()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.15.2");

                    case "/torrents/setShareLimits":
                        var form = await req.Content!.ReadAsStringAsync(ct);
                        form.Should().Contain("ratioLimit=");
                        form.Should().Contain("seedingTimeLimit=");
                        form.Should().Contain("inactiveSeedingTimeLimit=");
                        form.Should().Contain("shareLimitAction=1");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.SetTorrentShareLimitAsync(1, 2, 3, shareLimitAction: ShareLimitAction.Remove, all: true, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionWithoutAction_WHEN_SetTorrentShareLimit_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var setShareLimitsRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.2"));

                    case "/torrents/setShareLimits":
                        setShareLimitsRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.SetTorrentShareLimitAsync(1, 2, 3, shareLimitAction: null, all: true, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.15.2 requires shareLimitAction when setting share limits.");
            setShareLimitsRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_SetTorrentShareLimit_THEN_ShouldReturnProbeFailure()
        {
            var setShareLimitsRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/setShareLimits":
                        setShareLimitsRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.SetTorrentShareLimitAsync(1, 2, 3, shareLimitAction: ShareLimitAction.Remove, all: true, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");

            setShareLimitsRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_GetTorrentUploadLimit_THEN_ShouldReturnDictionary()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"x\":10}")
            });

            var result = (await _target.GetTorrentUploadLimitAsync(all: false, hashes: ["x"], cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result["x"].Should().Be(10);
        }

        [Fact]
        public async Task GIVEN_BadJson_WHEN_GetTorrentUploadLimit_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetTorrentUploadLimitAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_LimitAndHashes_WHEN_SetTorrentUploadLimit_THEN_ShouldPostLimitAndHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/setUploadLimit");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("hashes=h1&limit=42");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentUploadLimitAsync(42, all: false, hashes: ["h1"], cancellationToken: TestContext.Current.CancellationToken);
        }

        private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string? content)
        {
            if (content is null)
            {
                return new HttpResponseMessage(statusCode);
            }

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            };
        }
    }
}
