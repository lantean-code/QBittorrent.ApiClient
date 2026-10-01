using System.Net;
using AwesomeAssertions;
using Moq;
using QBittorrent.ApiClient.Models;

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
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/increasePrio");
                (await req.Content.ReadAsStringOrNullAsync(ct)).Should().Be("hashes=h1%7Ch2");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.IncreaseTorrentPriorityAsync(TorrentSelector.FromHashes(["h1", "h2"]), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_AllTrue_WHEN_DecreaseTorrentPriority_THEN_ShouldPostAll()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/decreasePrio");
                (await req.Content.ReadAsStringOrNullAsync(ct)).Should().Be("hashes=all");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DecreaseTorrentPriorityAsync(TorrentSelector.AllTorrents(), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_MaxTorrentPriority_THEN_ShouldPostHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/topPrio");
                (await req.Content.ReadAsStringOrNullAsync(ct)).Should().Be("hashes=h");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.MaxTorrentPriorityAsync(TorrentSelector.FromHash("h"), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_MinTorrentPriority_THEN_ShouldPostHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/bottomPrio");
                (await req.Content.ReadAsStringOrNullAsync(ct)).Should().Be("hashes=h1%7Ch2%7Ch3");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.MinTorrentPriorityAsync(TorrentSelector.FromHashes(["h1", "h2", "h3"]), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_FileIdsAndPriority_WHEN_SetFilePriority_THEN_ShouldPostIdsAndPriorityInt()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/filePrio");
                var body = await req.Content.ReadAsUnescapedStringOrNullAsync(ct);
                body.Should().Be("hash=h1&id=1|2|3&priority=7");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetFilePriorityAsync("h1", [1, 2, 3], (Priority)7, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_GetTorrentDownloadLimit_THEN_ShouldReturnDictionary()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"h1\":1000,\"h2\":0}")
            });

            var result = (await _target.GetTorrentDownloadLimitAsync(TorrentSelector.FromHashes(["h1", "h2"]), cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

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

            var result = await _target.GetTorrentDownloadLimitAsync(TorrentSelector.AllTorrents(), cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_LimitAndHashes_WHEN_SetTorrentDownloadLimit_THEN_ShouldPostLimitAndHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setDownloadLimit");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hashes=h%7Ci&limit=500");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentDownloadLimitAsync(TorrentSelector.FromHashes(["h", "i"]), 500, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeShareLimitActionRequirementAndActionOmitted_WHEN_SetTorrentShareLimit_THEN_ShouldOmitActionField()
        {
            var ratio = 1.5f.ToString();
            var seed = 2.ToString();
            var inactive = 3.ToString();

            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.10");

                    case "/torrents/setShareLimits":
                        var form = await req.Content.ReadAsStringOrNullAsync(ct);
                        form.Should().NotBeNull();
                        if (form is null)
                        {
                            return CreateResponse(HttpStatusCode.BadRequest, "");
                        }

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

            (await _target.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            (await _target.SetTorrentShareLimitAsync(
                TorrentSelector.FromHashes(["h1", "h2"]),
                ratioLimit: 1.5f,
                seedingTimeLimit: 2,
                inactiveSeedingTimeLimit: 3,
                cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionWithShareLimitActionRequirementAndActionOmitted_WHEN_SetTorrentShareLimit_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var setShareLimitsRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.12.0"));

                    case "/torrents/setShareLimits":
                        setShareLimitsRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            var result = await _target.SetTorrentShareLimitAsync(TorrentSelector.AllTorrents(), 1, 2, 3, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.12.0 requires shareLimitAction when setting share limits.");
            setShareLimitsRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndAction_WHEN_SetTorrentShareLimit_THEN_ShouldIncludeActionField()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.12.0");

                    case "/torrents/setShareLimits":
                        var form = await req.Content.ReadAsStringOrNullAsync(ct);
                        form.Should().Contain("ratioLimit=");
                        form.Should().Contain("seedingTimeLimit=");
                        form.Should().Contain("inactiveSeedingTimeLimit=");
                        form.Should().Contain("shareLimitAction=Remove");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            (await _target.SetTorrentShareLimitAsync(TorrentSelector.AllTorrents(), 1, 2, 3, shareLimitAction: ShareLimitAction.Remove, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndTypedLimitSentinels_WHEN_SetTorrentShareLimit_THEN_ShouldAllowTypedConstantsWithoutCasting()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.12.0");

                    case "/torrents/setShareLimits":
                        var form = await req.Content.ReadAsStringOrNullAsync(ct);
                        form.Should().Contain("ratioLimit=-2");
                        form.Should().Contain("seedingTimeLimit=-2");
                        form.Should().Contain("inactiveSeedingTimeLimit=-1");
                        form.Should().Contain("shareLimitAction=Remove");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            (await _target.SetTorrentShareLimitAsync(
                TorrentSelector.AllTorrents(),
                Limits.UseGlobalShareRatioLimit,
                Limits.UseGlobalSeedingTimeLimit,
                Limits.NoInactiveSeedingTimeLimit,
                shareLimitAction: ShareLimitAction.Remove,
                cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeShareLimitActionRequirementAndAction_WHEN_SetTorrentShareLimit_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var setShareLimitsRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.10"));

                    case "/torrents/setShareLimits":
                        setShareLimitsRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            var result = await _target.SetTorrentShareLimitAsync(TorrentSelector.AllTorrents(), 1, 2, 3, shareLimitAction: ShareLimitAction.Remove, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.10 does not support shareLimitAction when setting share limits.");
            setShareLimitsRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_SetTorrentShareLimit_THEN_ShouldThrowWhenClientIsNotInitialized()
        {
            var setShareLimitsRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
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

            var action = async () => await _target.SetTorrentShareLimitAsync(TorrentSelector.AllTorrents(), 1, 2, 3, shareLimitAction: ShareLimitAction.Remove, cancellationToken: TestContext.Current.CancellationToken);

            await action.ShouldThrowUninitializedCompatibilityExceptionAsync();

            setShareLimitsRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_GetTorrentUploadLimit_THEN_ShouldReturnDictionary()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"x\":10}")
            });

            var result = (await _target.GetTorrentUploadLimitAsync(TorrentSelector.FromHash("x"), cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

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

            var result = await _target.GetTorrentUploadLimitAsync(TorrentSelector.AllTorrents(), cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_LimitAndHashes_WHEN_SetTorrentUploadLimit_THEN_ShouldPostLimitAndHashes()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setUploadLimit");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hashes=h1&limit=42");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentUploadLimitAsync(TorrentSelector.FromHash("h1"), 42, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ShareLimitsModeSupportAndNoMode_WHEN_SetTorrentShareLimit_THEN_ShouldIncludeDefaultMode()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = async (request, cancellationToken) =>
            {
                (await request.Content.ReadAsUnescapedStringOrNullAsync(cancellationToken)).Should().Contain("shareLimitsMode=Default");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            (await _target.SetTorrentShareLimitAsync(TorrentSelector.AllTorrents(), 1, 2, 3, ShareLimitAction.Stop, TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ShareLimitsModeSupportAndMode_WHEN_SetTorrentShareLimit_THEN_ShouldIncludeRequestedMode()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = async (request, cancellationToken) =>
            {
                (await request.Content.ReadAsUnescapedStringOrNullAsync(cancellationToken)).Should().Contain("shareLimitsMode=MatchAll");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            (await _target.SetTorrentShareLimitAsync(TorrentSelector.AllTorrents(), 1, 2, 3, ShareLimitAction.Stop, ShareLimitsMode.MatchAll, TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiWithoutShareLimitsModeSupportAndMode_WHEN_SetTorrentShareLimit_THEN_ShouldReturnUnsupportedVersion()
        {
            _target.Initialize(new Version(2, 15, 1));

            var result = await _target.SetTorrentShareLimitAsync(TorrentSelector.AllTorrents(), 1, 2, 3, ShareLimitAction.Stop, ShareLimitsMode.MatchAny, TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
        }

        [Fact]
        public async Task GIVEN_NullSelector_WHEN_SetTorrentShareLimit_THEN_ShouldThrowArgumentNullException()
        {
            _target.Initialize(new Version(2, 16, 2));
            var action = async () => await _target.SetTorrentShareLimitAsync(null!, 1, 2, 3, ShareLimitAction.Stop, TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GIVEN_TorrentFileDownloadSupportAndFilePath_WHEN_DownloadTorrentFile_THEN_ShouldStreamContentToDestination()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = (request, _) =>
            {
                request.RequestUri?.ToString().Should().Be("http://localhost/torrents/downloadFile?hash=hash&file=folder%2Ffile.txt");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent([1, 2])
                });
            };
            using var destination = new MemoryStream();

            var result = await _target.DownloadTorrentFileAsync("hash", "folder/file.txt", destination, TestContext.Current.CancellationToken);

            result.ShouldSucceed();
            destination.ToArray().Should().Equal(1, 2);
        }

        [Fact]
        public async Task GIVEN_ApiWithoutTorrentFileDownloadSupport_WHEN_DownloadTorrentFile_THEN_ShouldReturnUnsupportedVersion()
        {
            _target.Initialize(new Version(2, 15, 1));
            using var destination = new MemoryStream();

            var result = await _target.DownloadTorrentFileAsync("hash", "0", destination, TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
        }

        [Theory]
        [InlineData("", "0")]
        [InlineData("hash", "")]
        public async Task GIVEN_EmptyRequiredValue_WHEN_DownloadTorrentFile_THEN_ShouldThrowArgumentException(string hash, string file)
        {
            using var destination = new MemoryStream();
            var action = async () => await _target.DownloadTorrentFileAsync(hash, file, destination, TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GIVEN_NullDestination_WHEN_DownloadTorrentFile_THEN_ShouldThrowArgumentNullException()
        {
            var action = async () => await _target.DownloadTorrentFileAsync("hash", "0", null!, TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GIVEN_UnwritableDestination_WHEN_DownloadTorrentFile_THEN_ShouldThrowArgumentException()
        {
            using var destination = new MemoryStream([], writable: false);
            var action = async () => await _target.DownloadTorrentFileAsync("hash", "0", destination, TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GIVEN_TorrentFileDownloadSupportAndNonSuccessResponse_WHEN_DownloadTorrentFile_THEN_ShouldReturnFailure()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("metadata unavailable")
            });
            using var destination = new MemoryStream();

            var result = await _target.DownloadTorrentFileAsync("hash", "0", destination, TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.Conflict, userMessage: "metadata unavailable");
            destination.Length.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ResponseBodyHttpRequestFailure_WHEN_DownloadTorrentFile_THEN_ShouldReturnNoResponseFailure()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent([1])
            });
            var destination = CreateFailingDestination(new HttpRequestException("body failed"));

            var result = await _target.DownloadTorrentFileAsync("hash", "0", destination.Object, TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.NoResponse, userMessage: "body failed");
        }

        [Fact]
        public async Task GIVEN_ResponseBodyIoFailure_WHEN_DownloadTorrentFile_THEN_ShouldReturnNoResponseFailure()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent([1])
            });
            var destination = CreateFailingDestination(new IOException("write failed"));

            var result = await _target.DownloadTorrentFileAsync("hash", "0", destination.Object, TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.NoResponse);
        }

        [Fact]
        public async Task GIVEN_ResponseBodyTimeout_WHEN_DownloadTorrentFile_THEN_ShouldReturnTimeoutFailure()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent([1])
            });
            var destination = CreateFailingDestination(new TaskCanceledException("body timed out"));

            var result = await _target.DownloadTorrentFileAsync("hash", "0", destination.Object, TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.Timeout);
        }

        [Fact]
        public async Task GIVEN_CanceledTokenDuringResponseBody_WHEN_DownloadTorrentFile_THEN_ShouldPropagateCancellation()
        {
            _target.Initialize(new Version(2, 16, 2));
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent([1])
            });
            using var cancellationTokenSource = new CancellationTokenSource();
            var destination = new Mock<Stream>();
            destination.SetupGet(stream => stream.CanWrite).Returns(true);
            destination
                .Setup(stream => stream.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), cancellationTokenSource.Token))
                .Returns(() =>
                {
                    cancellationTokenSource.Cancel();
                    return ValueTask.FromException(new OperationCanceledException(cancellationTokenSource.Token));
                });
            destination
                .Setup(stream => stream.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), cancellationTokenSource.Token))
                .Returns(() =>
                {
                    cancellationTokenSource.Cancel();
                    return Task.FromException(new OperationCanceledException(cancellationTokenSource.Token));
                });

            var action = async () => await _target.DownloadTorrentFileAsync("hash", "0", destination.Object, cancellationTokenSource.Token);

            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        private static Mock<Stream> CreateFailingDestination(Exception exception)
        {
            var destination = new Mock<Stream>();
            destination.SetupGet(stream => stream.CanWrite).Returns(true);
            destination
                .Setup(stream => stream.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromException(exception));
            destination
                .Setup(stream => stream.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return destination;
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
