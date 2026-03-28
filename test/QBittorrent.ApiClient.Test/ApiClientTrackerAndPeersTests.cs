using AwesomeAssertions;
using QBittorrent.ApiClient.Models;
using System.Net;

namespace QBittorrent.ApiClient.Test
{
    public sealed class ApiClientTrackerAndPeersTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTrackerAndPeersTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_NoHashesAndAllFalse_WHEN_AddTrackersToTorrent_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.AddTrackersToTorrentAsync(new[] { "udp://tracker.example.com:80/announce" }, false, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_NullHashesAndAllNull_WHEN_AddTrackersToTorrent_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.AddTrackersToTorrentAsync(new[] { "udp://tracker.example.com:80/announce" }, all: null, hashes: null!, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionAndAllTrue_WHEN_AddTrackersToTorrent_THEN_ShouldPostAllAndUrlList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.15.2");

                    case "/torrents/addTrackers":
                        request.RequestUri!.ToString().Should().Be("http://localhost/torrents/addTrackers");
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=all&urls=udp%3A%2F%2Fa%0Audp%3A%2F%2Fb");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(new[] { "udp://a", "udp://b" }, true, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndSingleHash_WHEN_AddTrackersToTorrent_THEN_ShouldPostSingleHashAndUrlList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.4");

                    case "/torrents/addTrackers":
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(new[] { "udp://a" }, all: false, hashes: ["hash1"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndAllTrue_WHEN_AddTrackersToTorrent_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var addTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/torrents/addTrackers":
                        addTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.AddTrackersToTorrentAsync(new[] { "udp://a" }, true, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.4 does not support adding trackers to all torrents in a single request.");
            addTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndMultipleHashes_WHEN_AddTrackersToTorrent_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var addTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/torrents/addTrackers":
                        addTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.AddTrackersToTorrentAsync(new[] { "udp://a" }, all: false, hashes: ["hash1", "hash2"], cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.4 does not support adding trackers to multiple torrents in a single request.");
            addTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionAndNonSuccess_WHEN_AddTrackersToTorrent_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.2"));

                    case "/torrents/addTrackers":
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("failed")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.AddTrackersToTorrentAsync(new[] { "udp://a" }, all: false, hashes: ["hash1"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "failed");
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionAndDefaultAll_WHEN_AddTrackersToTorrent_THEN_ShouldPostHashList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.15.2");

                    case "/torrents/addTrackers":
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(new[] { "udp://a" }, hashes: "hash1", cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_AddTrackersToTorrent_THEN_ShouldReturnProbeFailure()
        {
            var addTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/addTrackers":
                        addTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.AddTrackersToTorrentAsync(new[] { "udp://a" }, all: false, hashes: ["hash1"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");

            addTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_NoUpdateValues_WHEN_EditTracker_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.EditTrackerAsync("hash", "udp://old", null, null, cancellationToken: TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GIVEN_EmptyNewUrlWithoutTier_WHEN_EditTracker_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.EditTrackerAsync("hash", "udp://old", string.Empty, null, cancellationToken: TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GIVEN_ModernApiVersion_WHEN_EditTracker_THEN_ShouldPostModernFields()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.15.2");

                    case "/torrents/editTracker":
                        request.RequestUri!.ToString().Should().Be("http://localhost/torrents/editTracker");
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=hash&url=udp%3A%2F%2Fold&newUrl=udp%3A%2F%2Fnew&tier=2");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.EditTrackerAsync("hash", "udp://old", "udp://new", 2, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersion_WHEN_EditTracker_THEN_ShouldPostLegacyFields()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.4");

                    case "/torrents/editTracker":
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=hash&origUrl=udp%3A%2F%2Fold&newUrl=udp%3A%2F%2Fnew");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.EditTrackerAsync("hash", "udp://old", "udp://new", null, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndTier_WHEN_EditTracker_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var editTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/torrents/editTracker":
                        editTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.EditTrackerAsync("hash", "udp://old", null, 2, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.4 does not support editing tracker tiers.");
            editTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_EditTracker_THEN_ShouldReturnProbeFailure()
        {
            var editTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/editTracker":
                        editTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.EditTrackerAsync("hash", "udp://old", "udp://new", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");

            editTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_NoHashesAndAllFalse_WHEN_RemoveTrackers_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.RemoveTrackersAsync(new[] { "udp://tracker.example.com" }, false, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_NullHashesAndAllNull_WHEN_RemoveTrackers_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.RemoveTrackersAsync(new[] { "udp://tracker.example.com" }, all: null, hashes: null!, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionAndAllTrue_WHEN_RemoveTrackers_THEN_ShouldPostPipeSeparatedUrls()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.15.2");

                    case "/torrents/removeTrackers":
                        request.RequestUri!.ToString().Should().Be("http://localhost/torrents/removeTrackers");
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=all&urls=udp%3A%2F%2Fa%7Cudp%3A%2F%2Fb");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(new[] { "udp://a", "udp://b" }, true, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndAllTrue_WHEN_RemoveTrackers_THEN_ShouldUseLegacyAllValue()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.4");

                    case "/torrents/removeTrackers":
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=%2A&urls=udp%3A%2F%2Fa%7Cudp%3A%2F%2Fb");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(new[] { "udp://a", "udp://b" }, true, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndMultipleHashes_WHEN_RemoveTrackers_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var removeTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/torrents/removeTrackers":
                        removeTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.RemoveTrackersAsync(new[] { "udp://a" }, all: false, hashes: ["hash1", "hash2"], cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.4 does not support removing trackers from multiple torrents in a single request.");
            removeTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_LegacyApiVersionAndNonSuccess_WHEN_RemoveTrackers_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/torrents/removeTrackers":
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
                        {
                            Content = new StringContent("remove failed")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.RemoveTrackersAsync(new[] { "udp://a" }, all: false, hashes: ["hash1"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "remove failed");
        }

        [Fact]
        public async Task GIVEN_ModernApiVersionAndDefaultAll_WHEN_RemoveTrackers_THEN_ShouldPostHashList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.15.2");

                    case "/torrents/removeTrackers":
                        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(new[] { "udp://a" }, hashes: "hash1", cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_RemoveTrackers_THEN_ShouldReturnProbeFailure()
        {
            var removeTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/removeTrackers":
                        removeTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.RemoveTrackersAsync(new[] { "udp://a" }, all: false, hashes: ["hash1"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");

            removeTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_HashesAndPeers_WHEN_AddPeers_THEN_ShouldPostPipeSeparatedValues()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                request.RequestUri!.ToString().Should().Be("http://localhost/torrents/addPeers");
                var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                body.Should().Be("hashes=h1%7Ch2&peers=127.0.0.1%3A6881%7C127.0.0.2%3A6882");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            (await _target.AddPeersAsync(new[] { "h1", "h2" }, new[] { new PeerId("127.0.0.1", 6881), new PeerId("127.0.0.2", 6882) }, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
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
