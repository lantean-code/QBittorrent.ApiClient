using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

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
            var action = async () => await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHashes([]), ["udp://tracker.example.com:80/announce"], cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_NullHashesAndAllNull_WHEN_AddTrackersToTorrent_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHashes(null!), ["udp://tracker.example.com:80/announce"], cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndAllTrue_WHEN_AddTrackersToTorrent_THEN_ShouldPostAllAndUrlList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/addTrackers":
                        request.RequestUri?.ToString().Should().Be("http://localhost/torrents/addTrackers");
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=all&urls=udp%3A%2F%2Fa%0Audp%3A%2F%2Fb");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(TorrentSelector.AllTorrents(), ["udp://a", "udp://b"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerBatchOperationsAndSingleHash_WHEN_AddTrackersToTorrent_THEN_ShouldPostSingleHashAndUrlList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.8");

                    case "/torrents/addTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerBatchOperationsAndDefaultAllValue_WHEN_AddTrackersToTorrent_THEN_ShouldPostSingleHashAndUrlList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.8");

                    case "/torrents/addTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerBatchOperationsAndAllTrue_WHEN_AddTrackersToTorrent_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var addTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/addTrackers":
                        addTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.AddTrackersToTorrentAsync(TorrentSelector.AllTorrents(), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.8 does not support adding trackers to all torrents in a single request.");
            addTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerBatchOperationsAndMultipleHashes_WHEN_AddTrackersToTorrent_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var addTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/addTrackers":
                        addTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHashes(["hash1", "hash2"]), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.8 does not support adding trackers to multiple torrents in a single request.");
            addTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndNonSuccess_WHEN_AddTrackersToTorrent_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/addTrackers":
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("failed")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "failed");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndDefaultAll_WHEN_AddTrackersToTorrent_THEN_ShouldPostHashList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/addTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndMultipleHashes_WHEN_AddTrackersToTorrent_THEN_ShouldPostPipeSeparatedHashList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/addTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash1%7Chash2&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHashes(["hash1", "hash2"]), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_AddTrackersToTorrent_THEN_ShouldReturnProbeFailure()
        {
            var addTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
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

            var result = await _target.AddTrackersToTorrentAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken);

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
        public async Task GIVEN_ApiVersionWithTrackerTierEditing_WHEN_EditTracker_THEN_ShouldPostTierEditingFields()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.0");

                    case "/torrents/editTracker":
                        request.RequestUri?.ToString().Should().Be("http://localhost/torrents/editTracker");
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash&url=udp%3A%2F%2Fold&newUrl=udp%3A%2F%2Fnew&tier=2");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.EditTrackerAsync("hash", "udp://old", "udp://new", 2, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerTierEditing_WHEN_EditTracker_THEN_ShouldPostPreTierEditingFields()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.12.1");

                    case "/torrents/editTracker":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash&origUrl=udp%3A%2F%2Fold&newUrl=udp%3A%2F%2Fnew");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.EditTrackerAsync("hash", "udp://old", "udp://new", null, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerTierEditingAndTier_WHEN_EditTracker_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var editTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.12.1"));

                    case "/torrents/editTracker":
                        editTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.EditTrackerAsync("hash", "udp://old", null, 2, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.12.1 does not support editing tracker tiers.");
            editTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_EditTracker_THEN_ShouldReturnProbeFailure()
        {
            var editTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
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
            var action = async () => await _target.RemoveTrackersAsync(TorrentSelector.FromHashes([]), ["udp://tracker.example.com"], cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_NullHashesAndAllNull_WHEN_RemoveTrackers_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.RemoveTrackersAsync(TorrentSelector.FromHashes(null!), ["udp://tracker.example.com"], cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndAllTrue_WHEN_RemoveTrackers_THEN_ShouldPostPipeSeparatedUrls()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/removeTrackers":
                        request.RequestUri?.ToString().Should().Be("http://localhost/torrents/removeTrackers");
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=all&urls=udp%3A%2F%2Fa%7Cudp%3A%2F%2Fb");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(TorrentSelector.AllTorrents(), ["udp://a", "udp://b"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerAllValueAndAllTrue_WHEN_RemoveTrackers_THEN_ShouldUseAsteriskAllValue()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.8");

                    case "/torrents/removeTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=%2A&urls=udp%3A%2F%2Fa%7Cudp%3A%2F%2Fb");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(TorrentSelector.AllTorrents(), ["udp://a", "udp://b"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerBatchOperationsAndMultipleHashes_WHEN_RemoveTrackers_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var removeTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/removeTrackers":
                        removeTrackerRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.RemoveTrackersAsync(TorrentSelector.FromHashes(["hash1", "hash2"]), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.8 does not support removing trackers from multiple torrents in a single request.");
            removeTrackerRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerBatchOperationsAndNonSuccess_WHEN_RemoveTrackers_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/removeTrackers":
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
                        {
                            Content = new StringContent("remove failed")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            var result = await _target.RemoveTrackersAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "remove failed");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndDefaultAll_WHEN_RemoveTrackers_THEN_ShouldPostHashList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/removeTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTrackerBatchOperationsAndDefaultAllValue_WHEN_RemoveTrackers_THEN_ShouldPostSingleHashAndUrlList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.8");

                    case "/torrents/removeTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash1&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndMultipleHashes_WHEN_RemoveTrackers_THEN_ShouldPostPipeSeparatedHashList()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                switch (request.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/removeTrackers":
                        var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                        body.Should().Be("hash=hash1%7Chash2&urls=udp%3A%2F%2Fa");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {request.RequestUri}");
                }
            };

            (await _target.RemoveTrackersAsync(TorrentSelector.FromHashes(["hash1", "hash2"]), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_RemoveTrackers_THEN_ShouldReturnProbeFailure()
        {
            var removeTrackerRequestCount = 0;

            _handler.Responder = (request, _) =>
            {
                switch (request.RequestUri?.AbsolutePath)
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

            var result = await _target.RemoveTrackersAsync(TorrentSelector.FromHash("hash1"), ["udp://a"], cancellationToken: TestContext.Current.CancellationToken);

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
                request.RequestUri?.ToString().Should().Be("http://localhost/torrents/addPeers");
                var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                body.Should().Be("hashes=h1%7Ch2&peers=127.0.0.1%3A6881%7C127.0.0.2%3A6882");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            (await _target.AddPeersAsync(TorrentSelector.FromHashes(["h1", "h2"]), [new PeerId("127.0.0.1", 6881), new PeerId("127.0.0.2", 6882)], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_AllSelectorAndPeers_WHEN_AddPeers_THEN_ShouldPostAllSelectorAndPipeSeparatedPeers()
        {
            _handler.Responder = async (request, cancellationToken) =>
            {
                request.RequestUri?.ToString().Should().Be("http://localhost/torrents/addPeers");
                var body = await request.Content.ReadAsStringOrNullAsync(cancellationToken);
                body.Should().Be("hashes=all&peers=127.0.0.1%3A6881%7C127.0.0.2%3A6882");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            (await _target.AddPeersAsync(TorrentSelector.AllTorrents(), [new PeerId("127.0.0.1", 6881), new PeerId("127.0.0.2", 6882)], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
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
