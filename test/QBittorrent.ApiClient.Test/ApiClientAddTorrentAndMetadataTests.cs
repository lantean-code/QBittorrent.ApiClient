using System.Net;
using System.Text;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientAddTorrentAndMetadataTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientAddTorrentAndMetadataTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_OnlyUrls_WHEN_AddTorrent_THEN_ShouldPostMultipartWithUrlsNewlineSeparated()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/add");
                req.Content.Should().BeOfType<MultipartFormDataContent>();

                var parts = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject.ToList();
                parts.Count.Should().Be(1);

                var urlsPart = parts.Single();
                urlsPart.Headers.ContentDisposition?.Name.Should().Be("urls");
                (await urlsPart.ReadAsStringAsync(ct)).Should().Be("u1\nu2");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                };
            };

            var p = new AddTorrentParams
            {
                Urls = ["u1", "u2"]
            };

            var result = (await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_FilesAndOptions_WHEN_AddTorrent_THEN_ShouldIncludeAllExpectedParts()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/torrents/add":
                        req.Content.Should().BeOfType<MultipartFormDataContent>();

                        var parts = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject.ToList();

                        async Task<string> readAsync(string name)
                        {
                            var part = parts.Single(p => p.Headers.ContentDisposition?.Name == name);
                            return await part.ReadAsStringAsync(ct);
                        }

                        parts.Any(p => p.Headers.ContentDisposition?.Name == "torrents" &&
                                       p.Headers.ContentDisposition?.FileName == "a.torrent").Should().BeTrue();
                        parts.Any(p => p.Headers.ContentDisposition?.Name == "torrents" &&
                                       p.Headers.ContentDisposition?.FileName == "b.torrent").Should().BeTrue();

                        (await readAsync("skip_checking")).Should().Be("true");
                        (await readAsync("sequentialDownload")).Should().Be("false");
                        (await readAsync("firstLastPiecePrio")).Should().Be("true");
                        (await readAsync("addToTopOfQueue")).Should().Be("true");
                        (await readAsync("forced")).Should().Be("false");
                        (await readAsync("stopped")).Should().Be("true");
                        (await readAsync("savepath")).Should().Be("/save");
                        (await readAsync("downloadPath")).Should().Be("/dl");
                        (await readAsync("useDownloadPath")).Should().Be("true");
                        (await readAsync("category")).Should().Be("Movies");
                        (await readAsync("tags")).Should().Be("one,two");
                        (await readAsync("rename")).Should().Be("renamed");
                        (await readAsync("upLimit")).Should().Be("123");
                        (await readAsync("dlLimit")).Should().Be("456");
                        (await readAsync("ratioLimit")).Should().Be("1.5");
                        (await readAsync("seedingTimeLimit")).Should().Be("90");
                        (await readAsync("inactiveSeedingTimeLimit")).Should().Be("30");
                        (await readAsync("shareLimitAction")).Should().Be("Remove");
                        (await readAsync("autoTMM")).Should().Be("true");
                        (await readAsync("stopCondition")).Should().Be("FilesChecked");
                        (await readAsync("contentLayout")).Should().Be("Subfolder");
                        (await readAsync("downloader")).Should().Be("curl");
                        (await readAsync("filePriorities")).Should().Be("0,1");
                        (await readAsync("ssl_certificate")).Should().Be("cert");
                        (await readAsync("ssl_private_key")).Should().Be("key");
                        (await readAsync("ssl_dh_params")).Should().Be("dh");
                        (await readAsync("cookie")).Should().Be("sessionid=123");

                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("{}")
                        };

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var s1 = new MemoryStream(Encoding.UTF8.GetBytes("a"));
            using var s2 = new MemoryStream(Encoding.UTF8.GetBytes("b"));

            var p = new AddTorrentParams
            {
                Urls = null,
                Torrents = new Dictionary<string, Stream> { { "a.torrent", s1 }, { "b.torrent", s2 } },
                SkipChecking = true,
                SequentialDownload = false,
                FirstLastPiecePriority = true,
                AddToTopOfQueue = true,
                Forced = false,
                Stopped = true,
                SavePath = "/save",
                DownloadPath = "/dl",
                UseDownloadPath = true,
                Category = "Movies",
                Tags = ["one", "two"],
                RenameTorrent = "renamed",
                UploadLimit = 123,
                DownloadLimit = 456,
                RatioLimit = 1.5,
                SeedingTimeLimit = 90,
                InactiveSeedingTimeLimit = 30,
                ShareLimitAction = ShareLimitAction.Remove,
                AutoTorrentManagement = true,
                StopCondition = StopCondition.FilesChecked,
                ContentLayout = TorrentContentLayout.Subfolder,
                Downloader = "curl",
                FilePriorities = [(Priority)0, (Priority)1],
                SslCertificate = "cert",
                SslPrivateKey = "key",
                SslDhParams = "dh",
                Cookie = "sessionid=123"
            };

            var result = (await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_PreciseRatioLimit_WHEN_AddTorrent_THEN_ShouldSerializeInvariantDouble()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/add");
                req.Content.Should().BeOfType<MultipartFormDataContent>();

                var ratioLimitPart = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject
                    .Single(part => part.Headers.ContentDisposition?.Name == "ratioLimit");

                (await ratioLimitPart.ReadAsStringAsync(ct)).Should().Be("1.23456789012345");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                };
            };

            var parameters = new AddTorrentParams
            {
                Urls = ["u"],
                RatioLimit = 1.23456789012345
            };

            var result = (await _target.AddTorrentAsync(parameters, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeAddTorrentDownloaderAndDownloader_WHEN_AddTorrent_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var addRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.0"));

                    case "/torrents/add":
                        addRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.AddTorrentAsync(new AddTorrentParams
            {
                Urls = ["u1"],
                Downloader = "plugin"
            }, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.13.0 does not support add-torrent downloader selection.");
            addRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeAddTorrentFilePrioritiesAndFilePriorities_WHEN_AddTorrent_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var addRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/add":
                        addRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.AddTorrentAsync(new AddTorrentParams
            {
                Urls = ["u1"],
                FilePriorities = [Priority.DoNotDownload]
            }, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.8 does not support add-torrent file priorities.");
            addRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndFilePriorities_WHEN_AddTorrent_THEN_ShouldPostFilePriorities()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/add":
                        var parts = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject.ToList();
                        parts.Should().ContainSingle();
                        parts[0].Headers.ContentDisposition?.Name.Should().Be("filePriorities");
                        (await parts[0].ReadAsStringAsync(ct)).Should().Be("0,1");
                        return CreateResponse(HttpStatusCode.OK, "{}");

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.AddTorrentAsync(new AddTorrentParams
            {
                FilePriorities = [Priority.DoNotDownload, Priority.Normal]
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailureAndDownloader_WHEN_AddTorrent_THEN_ShouldReturnProbeFailure()
        {
            var addRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/add":
                        addRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.AddTorrentAsync(new AddTorrentParams
            {
                Urls = ["u1"],
                Downloader = "plugin"
            }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            addRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndDownloaderWithoutFilePriorities_WHEN_AddTorrent_THEN_ShouldPostDownloaderWithoutProbingFailure()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/torrents/add":
                        var parts = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject.ToList();
                        parts.Should().ContainSingle();
                        parts[0].Headers.ContentDisposition?.Name.Should().Be("downloader");
                        (await parts[0].ReadAsStringAsync(ct)).Should().Be("plugin");
                        return CreateResponse(HttpStatusCode.OK, "{}");

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.AddTorrentAsync(new AddTorrentParams
            {
                Downloader = "plugin"
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_CallerOwnedTorrentStream_WHEN_AddTorrent_THEN_ShouldLeaveStreamReadableAtOriginalPosition()
        {
            _handler.Responder = async (req, ct) =>
            {
                var torrentPart = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject.Single(
                    p => p.Headers.ContentDisposition?.Name == "torrents");

                (await torrentPart.ReadAsStringAsync(ct)).Should().Be("bc");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                };
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc"));
            stream.Position = 1;

            var parameters = new AddTorrentParams
            {
                Torrents = new Dictionary<string, Stream>
                {
                    { "a.torrent", stream }
                }
            };

            var result = (await _target.AddTorrentAsync(parameters, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            stream.CanRead.Should().BeTrue();
            stream.Position.Should().Be(1);
            stream.ReadByte().Should().Be((byte)'b');
        }

        [Fact]
        public async Task GIVEN_CanceledToken_WHEN_AddTorrent_THEN_ShouldThrowOperationCanceledException()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc"));

            var parameters = new AddTorrentParams
            {
                Torrents = new Dictionary<string, Stream>
                {
                    { "a.torrent", stream }
                }
            };

            var action = async () => await _target.AddTorrentAsync(parameters, cancellationTokenSource.Token);

            await action.Should().ThrowAsync<OperationCanceledException>();
            stream.CanRead.Should().BeTrue();
        }

        [Fact]
        public async Task GIVEN_UnreadableTorrentStream_WHEN_AddTorrent_THEN_ShouldThrowArgumentException()
        {
            var parameters = new AddTorrentParams
            {
                Torrents = new Dictionary<string, Stream>
                {
                    { "a.torrent", new UnreadableStream() }
                }
            };

            var action = async () => await _target.AddTorrentAsync(parameters, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("stream");
        }

        [Fact]
        public async Task GIVEN_ConflictAndEmptyMessage_WHEN_AddTorrent_THEN_ShouldThrowWithDefaultConflictMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent(string.Empty)
            });

            var p = new AddTorrentParams { Urls = ["u"] };

            var result = await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "All torrents failed to add.");
        }

        [Fact]
        public async Task GIVEN_ConflictWithMessage_WHEN_AddTorrent_THEN_ShouldThrowWithServerMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("some failed")
            });

            var p = new AddTorrentParams { Urls = ["u"] };

            var result = await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "some failed");
        }

        [Fact]
        public async Task GIVEN_BadRequest_WHEN_AddTorrent_THEN_ShouldReturnValidationFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("invalid")
            });

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ValidationFailed,
                statusCode: HttpStatusCode.BadRequest,
                userMessage: "invalid");
        }

        [Fact]
        public async Task GIVEN_BadRequestWithoutBody_WHEN_AddTorrent_THEN_ShouldUseDefaultValidationMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.ValidationFailed,
                statusCode: HttpStatusCode.BadRequest,
                userMessage: "The torrent request was rejected.");

            failure.TryGetReason<AddTorrentFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(AddTorrentFailureReason.ValidationFailed);
        }

        [Fact]
        public async Task GIVEN_UnsupportedMediaType_WHEN_AddTorrent_THEN_ShouldReturnUnsupportedDataFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType)
            {
                Content = new StringContent("unsupported")
            });

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnsupportedData,
                statusCode: HttpStatusCode.UnsupportedMediaType,
                userMessage: "unsupported");
        }

        [Fact]
        public async Task GIVEN_UnsupportedMediaTypeWithoutBody_WHEN_AddTorrent_THEN_ShouldUseDefaultUnsupportedDataMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType));

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.UnsupportedData,
                statusCode: HttpStatusCode.UnsupportedMediaType,
                userMessage: "The supplied torrent data is not supported.");

            failure.TryGetReason<AddTorrentFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(AddTorrentFailureReason.InvalidTorrentData);
        }

        [Fact]
        public async Task GIVEN_UnexpectedStatusWithoutBody_WHEN_AddTorrent_THEN_ShouldUseGenericUnexpectedMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage((HttpStatusCode)418));

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                statusCode: (HttpStatusCode)418,
                userMessage: "Unexpected API response (418).");
        }

        [Fact]
        public async Task GIVEN_SuccessAndEmptyBody_WHEN_AddTorrent_THEN_ShouldReturnDefaultResultObject()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty)
            });

            var p = new AddTorrentParams { Urls = ["u"] };

            var result = (await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_TextOkPayload_WHEN_AddTorrent_THEN_ShouldReturnSingleSuccess()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Ok.")
            });

            var result = (await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(1);
            result.FailureCount.Should().Be(0);
            result.PendingCount.Should().Be(0);
            result.AddedTorrentIds.Should().BeEmpty();
        }

        [Fact]
        public async Task GIVEN_TextFailsPayload_WHEN_AddTorrent_THEN_ShouldReturnSingleFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Fails.")
            });

            var result = (await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(1);
            result.PendingCount.Should().Be(0);
            result.AddedTorrentIds.Should().BeEmpty();
        }

        [Fact]
        public async Task GIVEN_AsyncResultPayload_WHEN_AddTorrent_THEN_ShouldDeserializeAllResultFields()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                        "success_count": 1,
                        "failure_count": 2,
                        "pending_count": 3,
                        "added_torrent_ids": [ "hash1", "hash2" ]
                    }
                    """)
            });

            var result = (await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(1);
            result.FailureCount.Should().Be(2);
            result.PendingCount.Should().Be(3);
            result.AddedTorrentIds.Should().BeEquivalentTo(["hash1", "hash2"]);
        }

        [Fact]
        public async Task GIVEN_AcceptedAsyncResultPayload_WHEN_AddTorrent_THEN_ShouldReturnPendingResult()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent("""
                    {
                        "success_count": 1,
                        "failure_count": 2,
                        "pending_count": 3,
                        "added_torrent_ids": [ "hash1", "hash2" ]
                    }
                    """)
            });

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            var pendingResult = result.GetPendingValueOrThrow();

            pendingResult.SuccessCount.Should().Be(1);
            pendingResult.FailureCount.Should().Be(2);
            pendingResult.PendingCount.Should().Be(3);
            pendingResult.AddedTorrentIds.Should().BeEquivalentTo(["hash1", "hash2"]);
        }

        [Fact]
        public async Task GIVEN_AcceptedInvalidJson_WHEN_AddTorrent_THEN_ShouldReturnUnexpectedResponseFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent("not-json")
            });

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NullJsonPayload_WHEN_AddTorrent_THEN_ShouldFallbackToFailureCountFromInputSize()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            });

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("x"));
            var parameters = new AddTorrentParams
            {
                Urls = ["u1", "u2"],
                Torrents = new Dictionary<string, Stream> { ["file.torrent"] = stream }
            };

            var result = (await _target.AddTorrentAsync(parameters, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(3);
        }

        [Fact]
        public async Task GIVEN_NullJsonPayloadAndOnlyUrls_WHEN_AddTorrent_THEN_ShouldFallbackToUrlFailureCount()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            });

            var parameters = new AddTorrentParams
            {
                Urls = ["u1", "u2"]
            };

            var result = (await _target.AddTorrentAsync(parameters, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(2);
        }

        [Fact]
        public async Task GIVEN_NullJsonPayloadAndOnlyTorrents_WHEN_AddTorrent_THEN_ShouldFallbackToTorrentFailureCount()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            });

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("x"));
            var parameters = new AddTorrentParams
            {
                Torrents = new Dictionary<string, Stream> { ["file.torrent"] = stream }
            };

            var result = (await _target.AddTorrentAsync(parameters, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(1);
        }

        [Fact]
        public async Task GIVEN_NullJsonPayloadAndNoInputs_WHEN_AddTorrent_THEN_ShouldFallbackToZeroFailureCount()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            });

            var result = (await _target.AddTorrentAsync(new AddTorrentParams(), cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_RequestException_WHEN_AddTorrent_THEN_ShouldReturnNoResponseFailure()
        {
            _handler.Responder = (_, _) => throw new HttpRequestException("add failed");

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = ["u"] }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.NoResponse,
                userMessage: "add failed");
        }

        [Fact]
        public async Task GIVEN_BaseAddressAndHash_WHEN_GetExportUrl_THEN_ShouldReturnFormattedUrl()
        {
            var result = (await _target.GetExportUrlAsync("abc123")).GetValueOrThrow();

            result.Should().Be("http://localhost/torrents/export?hash=abc123");
        }

        [Fact]
        public async Task GIVEN_BaseAddressWithoutTrailingSlashAndSpecialHash_WHEN_GetExportUrl_THEN_ShouldPreserveBasePathAndEncodeHash()
        {
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/api/v2") };
            var target = new ApiClient(http);

            var result = (await target.GetExportUrlAsync("abc 123/+=")).GetValueOrThrow();

            result.Should().Be("http://localhost/api/v2/torrents/export?hash=abc%20123%2F%2B%3D");
        }

        [Fact]
        public async Task GIVEN_MissingBaseAddress_WHEN_GetExportUrl_THEN_ShouldReturnConfigurationFailure()
        {
            var target = new ApiClient(new HttpClient(_handler));

            var result = await target.GetExportUrlAsync("abc123");

            result.ShouldFailWith(
                kind: ApiFailureKind.InvalidConfiguration,
                userMessage: "HttpClient BaseAddress must be configured.");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_ExportTorrent_THEN_ShouldReturnTorrentBytes()
        {
            var expected = Encoding.UTF8.GetBytes("torrent-bytes");

            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/export?hash=abc123");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(expected)
                });
            };

            var result = (await _target.ExportTorrentAsync("abc123", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Equal(expected);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_ExportTorrent_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.ExportTorrentAsync("abc123", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_FetchTorrentMetadata_THEN_ShouldReturnResolvedMetadata()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/fetchMetadata":
                        req.Method.Should().Be(HttpMethod.Post);
                        (await req.Content.ReadAsStringOrNullAsync(ct)).Should().Be("source=magnet%3A%3Fxt%3Durn%3Abtih%3Aabc&downloader=plugin");
                        return CreateResponse(
                            HttpStatusCode.OK,
                            """
                            {
                                "infohash_v1": "InfoHashV1",
                                "infohash_v2": "InfoHashV2",
                                "hash": "Hash",
                                "created_by": "CreatedBy",
                                "creation_date": 946684800,
                                "comment": "Comment",
                                "trackers":
                                [
                                    {
                                        "url": "udp://tracker",
                                        "tier": 0
                                    }
                                ],
                                "webseeds": [ "https://seed" ],
                                "info":
                                {
                                    "name": "Name",
                                    "length": 99,
                                    "piece_length": 16,
                                    "pieces_num": 7,
                                    "private": true,
                                    "files":
                                    [
                                        {
                                            "path": "file.bin",
                                            "length": 99
                                        }
                                    ]
                                }
                            }
                            """);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.FetchTorrentMetadataAsync("magnet:?xt=urn:btih:abc", "plugin", cancellationToken: TestContext.Current.CancellationToken)).GetSuccessValueOrThrow();

            result.InfoHashV1.Should().Be("InfoHashV1");
            result.InfoHashV2.Should().Be("InfoHashV2");
            result.Hash.Should().Be("Hash");
            result.Info.Name.Should().Be("Name");
            result.Info.Length.Should().Be(99);
            result.Info.PieceLength.Should().Be(16);
            result.Info.PiecesNum.Should().Be(7);
            result.Info.Private.Should().BeTrue();
            result.CreatedBy.Should().Be("CreatedBy");
            result.CreationDate.Should().Be(946684800);
            result.Comment.Should().Be("Comment");
            result.Info.Files.Should().ContainSingle();
            result.Info.Files[0].Path.Should().Be("file.bin");
            result.Info.Files[0].Length.Should().Be(99);
            result.Trackers.Should().ContainSingle();
            result.Trackers[0].Url.Should().Be("udp://tracker");
            result.Trackers[0].Tier.Should().Be(0);
            result.WebSeeds.Should().ContainSingle().Which.Should().Be("https://seed");
        }

        [Fact]
        public async Task GIVEN_AcceptedResponseWithIdentifiers_WHEN_FetchTorrentMetadata_THEN_ShouldReturnPendingResult()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(
                            HttpStatusCode.Accepted,
                            """
                            {
                                "infohash_v1": "InfoHashV1",
                                "infohash_v2": "InfoHashV2",
                                "hash": "Hash"
                            }
                            """));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            var pendingResult = result.GetPendingValueOrThrow();

            pendingResult.InfoHashV1.Should().Be("InfoHashV1");
            pendingResult.InfoHashV2.Should().Be("InfoHashV2");
            pendingResult.Hash.Should().Be("Hash");
        }

        [Fact]
        public async Task GIVEN_AcceptedEmptyObject_WHEN_FetchTorrentMetadata_THEN_ShouldReturnPendingResultWithEmptyValues()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.Accepted, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            var pendingResult = result.GetPendingValueOrThrow();

            pendingResult.InfoHashV1.Should().BeNull();
            pendingResult.InfoHashV2.Should().BeNull();
            pendingResult.Hash.Should().BeNull();
        }

        [Fact]
        public async Task GIVEN_AcceptedNullPayload_WHEN_FetchTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.Accepted, "null"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_AcceptedInvalidJson_WHEN_FetchTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.Accepted, "not-json"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_RequestException_WHEN_FetchTorrentMetadata_THEN_ShouldReturnNoResponseFailure()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        throw new HttpRequestException("fetch failed");

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.NoResponse, userMessage: "fetch failed");
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTorrentMetadata_WHEN_FetchTorrentMetadata_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var metadataRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/fetchMetadata":
                        metadataRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.8 does not support torrent metadata APIs.");
            metadataRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_FetchTorrentMetadata_THEN_ShouldReturnProbeFailure()
        {
            var metadataRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/fetchMetadata":
                        metadataRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            metadataRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_FetchTorrentMetadata_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.NotFound, "missing"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_InvalidJson_WHEN_FetchTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "not-json"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_InvalidTrackerPayload_WHEN_FetchTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(
                            HttpStatusCode.OK,
                            """
                            {
                                "trackers":
                                [
                                    {
                                        "tier": 0
                                    }
                                ],
                                "info":
                                {
                                    "name": "Name",
                                    "length": 99,
                                    "piece_length": 16,
                                    "pieces_num": 7,
                                    "private": true,
                                    "files":
                                    [
                                        {
                                            "path": "file.bin",
                                            "length": 99
                                        }
                                    ]
                                }
                            }
                            """));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NullJsonPayload_WHEN_FetchTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/fetchMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "null"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.FetchTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_WebApi2130MultipartFiles_WHEN_ParseTorrentMetadata_THEN_ShouldReturnMetadataInResponseOrder()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.0");

                    case "/torrents/parseMetadata":
                        req.Method.Should().Be(HttpMethod.Post);
                        req.Content.Should().BeOfType<MultipartFormDataContent>();

                        var parts = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject.ToList();
                        parts.Count.Should().Be(2);
                        parts[0].Headers.ContentDisposition?.Name.Should().Be("torrents");
                        parts[0].Headers.ContentDisposition?.FileName.Should().Be("a.torrent");
                        parts[1].Headers.ContentDisposition?.FileName.Should().Be("b.torrent");
                        (await parts[0].ReadAsStringAsync(ct)).Should().Be("a");
                        (await parts[1].ReadAsStringAsync(ct)).Should().Be("b");

                        return CreateResponse(
                            HttpStatusCode.OK,
                            """
                            [
                                {
                                    "hash": "Hash1",
                                    "info":
                                    {
                                        "name": "First",
                                        "length": 10,
                                        "piece_length": 2,
                                        "pieces_num": 5,
                                        "private": false,
                                        "files":
                                        [
                                            {
                                                "path": "first.bin",
                                                "length": 10
                                            }
                                        ]
                                    }
                                },
                                {
                                    "hash": "Hash2",
                                    "info":
                                    {
                                        "name": "Second",
                                        "length": 20,
                                        "piece_length": 4,
                                        "pieces_num": 5,
                                        "private": true,
                                        "files":
                                        [
                                            {
                                                "path": "second.bin",
                                                "length": 20
                                            }
                                        ]
                                    }
                                }
                            ]
                            """);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var first = new MemoryStream(Encoding.UTF8.GetBytes("a"));
            using var second = new MemoryStream(Encoding.UTF8.GetBytes("b"));

            var result = (await _target.ParseTorrentMetadataAsync(
                new Dictionary<string, Stream>
                {
                    ["a.torrent"] = first,
                    ["b.torrent"] = second
                },
                cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Select(item => item.Info.Name).Should().Equal("First", "Second");
            result.Select(item => item.Hash).Should().Equal("Hash1", "Hash2");
        }

        [Fact]
        public async Task GIVEN_WebApi2121ObjectPayload_WHEN_ParseTorrentMetadata_THEN_ShouldReturnMetadataInRequestOrder()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.12.1");

                    case "/torrents/parseMetadata":
                        req.Method.Should().Be(HttpMethod.Post);
                        req.Content.Should().BeOfType<MultipartFormDataContent>();

                        var parts = req.Content.Should().BeOfType<MultipartFormDataContent>().Subject.ToList();
                        parts.Count.Should().Be(2);
                        parts[0].Headers.ContentDisposition?.Name.Should().Be("torrents");
                        parts[0].Headers.ContentDisposition?.FileName.Should().Be("a.torrent");
                        parts[1].Headers.ContentDisposition?.FileName.Should().Be("b.torrent");
                        (await parts[0].ReadAsStringAsync(ct)).Should().Be("a");
                        (await parts[1].ReadAsStringAsync(ct)).Should().Be("b");

                        return CreateResponse(
                            HttpStatusCode.OK,
                            """
                            {
                                "b.torrent":
                                {
                                    "hash": "Hash2",
                                    "info":
                                    {
                                        "name": "Second",
                                        "length": 20,
                                        "piece_length": 4,
                                        "pieces_num": 5,
                                        "private": true,
                                        "files":
                                        [
                                            {
                                                "path": "second.bin",
                                                "length": 20
                                            }
                                        ]
                                    }
                                },
                                "a.torrent":
                                {
                                    "hash": "Hash1",
                                    "info":
                                    {
                                        "name": "First",
                                        "length": 10,
                                        "piece_length": 2,
                                        "pieces_num": 5,
                                        "private": false,
                                        "files":
                                        [
                                            {
                                                "path": "first.bin",
                                                "length": 10
                                            }
                                        ]
                                    }
                                }
                            }
                            """);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var first = new MemoryStream(Encoding.UTF8.GetBytes("a"));
            using var second = new MemoryStream(Encoding.UTF8.GetBytes("b"));

            var result = (await _target.ParseTorrentMetadataAsync(
                new Dictionary<string, Stream>
                {
                    ["a.torrent"] = first,
                    ["b.torrent"] = second
                },
                cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Select(item => item.Info.Name).Should().Equal("First", "Second");
            result.Select(item => item.Hash).Should().Equal("Hash1", "Hash2");
        }

        [Fact]
        public async Task GIVEN_WebApi2121ObjectPayloadMissingRequestedEntry_WHEN_ParseTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.12.1"));

                    case "/torrents/parseMetadata":
                        return Task.FromResult(CreateResponse(
                            HttpStatusCode.OK,
                            """
                            {
                                "a.torrent":
                                {
                                    "hash": "Hash1",
                                    "info":
                                    {
                                        "name": "First",
                                        "length": 10,
                                        "piece_length": 2,
                                        "pieces_num": 5,
                                        "private": false,
                                        "files":
                                        [
                                            {
                                                "path": "first.bin",
                                                "length": 10
                                            }
                                        ]
                                    }
                                }
                            }
                            """));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var first = new MemoryStream(Encoding.UTF8.GetBytes("a"));
            using var second = new MemoryStream(Encoding.UTF8.GetBytes("b"));

            var result = await _target.ParseTorrentMetadataAsync(
                new Dictionary<string, Stream>
                {
                    ["a.torrent"] = first,
                    ["b.torrent"] = second
                },
                cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NullTorrents_WHEN_ParseTorrentMetadata_THEN_ShouldThrowArgumentNullException()
        {
            var action = async () => await _target.ParseTorrentMetadataAsync(null!, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("torrents");
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTorrentMetadata_WHEN_ParseTorrentMetadata_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var metadataRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/parseMetadata":
                        metadataRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "[]"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a"));

            var result = await _target.ParseTorrentMetadataAsync(new Dictionary<string, Stream> { ["a.torrent"] = stream }, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.8 does not support torrent metadata APIs.");
            metadataRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_ParseTorrentMetadata_THEN_ShouldReturnProbeFailure()
        {
            var metadataRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/parseMetadata":
                        metadataRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "[]"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a"));

            var result = await _target.ParseTorrentMetadataAsync(new Dictionary<string, Stream> { ["a.torrent"] = stream }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            metadataRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ObjectPayloadFromArrayResponseVersion_WHEN_ParseTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.0"));

                    case "/torrents/parseMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a"));

            var result = await _target.ParseTorrentMetadataAsync(new Dictionary<string, Stream> { ["a.torrent"] = stream }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_MetadataEntryWithoutInfo_WHEN_ParseTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.0"));

                    case "/torrents/parseMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, """[{ "hash": "Hash1" }]"""));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a"));

            var result = await _target.ParseTorrentMetadataAsync(new Dictionary<string, Stream> { ["a.torrent"] = stream }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_InvalidMetadataEntry_WHEN_ParseTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.0"));

                    case "/torrents/parseMetadata":
                        return Task.FromResult(CreateResponse(
                            HttpStatusCode.OK,
                            """
                            [
                                {
                                    "info":
                                    {
                                        "name": "Name",
                                        "length": "invalid",
                                        "piece_length": 16,
                                        "pieces_num": 7,
                                        "private": true,
                                        "files": []
                                    }
                                }
                            ]
                            """));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a"));

            var result = await _target.ParseTorrentMetadataAsync(new Dictionary<string, Stream> { ["a.torrent"] = stream }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_InvalidFileEntry_WHEN_ParseTorrentMetadata_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.0"));

                    case "/torrents/parseMetadata":
                        return Task.FromResult(CreateResponse(
                            HttpStatusCode.OK,
                            """
                            [
                                {
                                    "info":
                                    {
                                        "name": "Name",
                                        "length": 10,
                                        "piece_length": 2,
                                        "pieces_num": 5,
                                        "private": false,
                                        "files":
                                        [
                                            {
                                                "length": 10
                                            }
                                        ]
                                    }
                                }
                            ]
                            """));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a"));

            var result = await _target.ParseTorrentMetadataAsync(new Dictionary<string, Stream> { ["a.torrent"] = stream }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse, userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_SaveTorrentMetadata_THEN_ShouldReturnTorrentBytes()
        {
            var expected = Encoding.UTF8.GetBytes("saved-torrent");

            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.9");

                    case "/torrents/saveMetadata":
                        req.Method.Should().Be(HttpMethod.Post);
                        (await req.Content.ReadAsStringOrNullAsync(ct)).Should().Be("source=source");
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(expected)
                        };

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.SaveTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Equal(expected);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTorrentMetadata_WHEN_SaveTorrentMetadata_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var metadataRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/saveMetadata":
                        metadataRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.SaveTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.8 does not support torrent metadata APIs.");
            metadataRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_SaveTorrentMetadata_THEN_ShouldReturnProbeFailure()
        {
            var metadataRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/saveMetadata":
                        metadataRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.SaveTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            metadataRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SaveTorrentMetadata_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.9"));

                    case "/torrents/saveMetadata":
                        return Task.FromResult(CreateResponse(HttpStatusCode.NotFound, "missing"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.SaveTorrentMetadataAsync("source", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
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

        private sealed class UnreadableStream : Stream
        {
            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get
                {
                    throw new NotSupportedException();
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
