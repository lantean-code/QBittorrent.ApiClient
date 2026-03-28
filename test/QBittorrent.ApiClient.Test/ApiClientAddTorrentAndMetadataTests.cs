using AwesomeAssertions;
using QBittorrent.ApiClient.Models;
using System.Net;
using System.Text;

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
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/add");
                req.Content.Should().BeOfType<MultipartFormDataContent>();

                var parts = (req.Content as MultipartFormDataContent)!.ToList();
                parts.Count.Should().Be(1);

                var urlsPart = parts.Single();
                urlsPart.Headers.ContentDisposition!.Name.Should().Be("urls");
                (await urlsPart.ReadAsStringAsync()).Should().Be("u1\nu2");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                };
            };

            var p = new AddTorrentParams
            {
                Urls = new[] { "u1", "u2" }
            };

            var result = (await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_FilesAndOptions_WHEN_AddTorrent_THEN_ShouldIncludeAllExpectedParts()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/add");
                req.Content.Should().BeOfType<MultipartFormDataContent>();

                var parts = (req.Content as MultipartFormDataContent)!.ToList();

                async Task<string> ReadAsync(string name)
                {
                    var part = parts.Single(p => p.Headers.ContentDisposition!.Name == name);
                    return await part.ReadAsStringAsync(ct);
                }

                parts.Any(p => p.Headers.ContentDisposition!.Name == "torrents" &&
                               p.Headers.ContentDisposition!.FileName == "a.torrent").Should().BeTrue();
                parts.Any(p => p.Headers.ContentDisposition!.Name == "torrents" &&
                               p.Headers.ContentDisposition!.FileName == "b.torrent").Should().BeTrue();

                (await ReadAsync("skip_checking")).Should().Be("true");
                (await ReadAsync("sequentialDownload")).Should().Be("false");
                (await ReadAsync("firstLastPiecePrio")).Should().Be("true");
                (await ReadAsync("addToTopOfQueue")).Should().Be("true");
                (await ReadAsync("forced")).Should().Be("false");
                (await ReadAsync("stopped")).Should().Be("true");
                (await ReadAsync("savepath")).Should().Be("/save");
                (await ReadAsync("downloadPath")).Should().Be("/dl");
                (await ReadAsync("useDownloadPath")).Should().Be("true");
                (await ReadAsync("category")).Should().Be("Movies");
                (await ReadAsync("tags")).Should().Be("one,two");
                (await ReadAsync("rename")).Should().Be("renamed");
                (await ReadAsync("upLimit")).Should().Be("123");
                (await ReadAsync("dlLimit")).Should().Be("456");
                (await ReadAsync("ratioLimit")).Should().Be("1.5");
                (await ReadAsync("seedingTimeLimit")).Should().Be("90");
                (await ReadAsync("inactiveSeedingTimeLimit")).Should().Be("30");
                (await ReadAsync("shareLimitAction")).Should().Be("Remove");
                (await ReadAsync("autoTMM")).Should().Be("true");
                (await ReadAsync("stopCondition")).Should().Be("FilesChecked");
                (await ReadAsync("contentLayout")).Should().Be("Subfolder");
                (await ReadAsync("downloader")).Should().Be("curl");
                (await ReadAsync("filePriorities")).Should().Be("0,1");
                (await ReadAsync("ssl_certificate")).Should().Be("cert");
                (await ReadAsync("ssl_private_key")).Should().Be("key");
                (await ReadAsync("ssl_dh_params")).Should().Be("dh");
                (await ReadAsync("cookie")).Should().Be("sessionid=123");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                };
            };

            using var s1 = new MemoryStream(Encoding.UTF8.GetBytes("a"));
            using var s2 = new MemoryStream(Encoding.UTF8.GetBytes("b"));

            var p = new AddTorrentParams
            {
                Urls = null,
                Torrents = new Dictionary<string, Stream> { { "a.torrent", (Stream)s1 }, { "b.torrent", (Stream)s2 } },
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
                Tags = new[] { "one", "two" },
                RenameTorrent = "renamed",
                UploadLimit = 123,
                DownloadLimit = 456,
                RatioLimit = 1.5f,
                SeedingTimeLimit = 90,
                InactiveSeedingTimeLimit = 30,
                ShareLimitAction = ShareLimitAction.Remove,
                AutoTorrentManagement = true,
                StopCondition = StopCondition.FilesChecked,
                ContentLayout = TorrentContentLayout.Subfolder,
                Downloader = "curl",
                FilePriorities = new[] { (Priority)0, (Priority)1 },
                SslCertificate = "cert",
                SslPrivateKey = "key",
                SslDhParams = "dh",
                Cookie = "sessionid=123"
            };

            var result = (await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_CallerOwnedTorrentStream_WHEN_AddTorrent_THEN_ShouldLeaveStreamReadableAtOriginalPosition()
        {
            _handler.Responder = async (req, _) =>
            {
                var torrentPart = (req.Content as MultipartFormDataContent)!.Single(
                    p => p.Headers.ContentDisposition!.Name == "torrents");

                (await torrentPart.ReadAsStringAsync()).Should().Be("bc");

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
        public async Task GIVEN_ConflictAndEmptyMessage_WHEN_AddTorrent_THEN_ShouldThrowWithDefaultConflictMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent(string.Empty)
            });

            var p = new AddTorrentParams { Urls = new[] { "u" } };

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

            var p = new AddTorrentParams { Urls = new[] { "u" } };

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

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ValidationFailed,
                statusCode: HttpStatusCode.BadRequest,
                userMessage: "invalid");
        }

        [Fact]
        public async Task GIVEN_BadRequestWithoutBody_WHEN_AddTorrent_THEN_ShouldUseDefaultValidationMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken);

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

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnsupportedData,
                statusCode: HttpStatusCode.UnsupportedMediaType,
                userMessage: "unsupported");
        }

        [Fact]
        public async Task GIVEN_UnsupportedMediaTypeWithoutBody_WHEN_AddTorrent_THEN_ShouldUseDefaultUnsupportedDataMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType));

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken);

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

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken);

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

            var p = new AddTorrentParams { Urls = new[] { "u" } };

            var result = (await _target.AddTorrentAsync(p, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_LegacyOkPayload_WHEN_AddTorrent_THEN_ShouldReturnSingleSuccess()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Ok.")
            });

            var result = (await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(1);
            result.FailureCount.Should().Be(0);
            result.SupportsAsync.Should().BeFalse();
        }

        [Fact]
        public async Task GIVEN_LegacyFailsPayload_WHEN_AddTorrent_THEN_ShouldReturnSingleFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Fails.")
            });

            var result = (await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(1);
            result.SupportsAsync.Should().BeFalse();
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

            var result = (await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.SuccessCount.Should().Be(1);
            result.FailureCount.Should().Be(2);
            result.PendingCount.Should().Be(3);
            result.AddedTorrentIds.Should().BeEquivalentTo(new[] { "hash1", "hash2" });
            result.SupportsAsync.Should().BeTrue();
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
                Urls = new[] { "u1", "u2" },
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
                Urls = new[] { "u1", "u2" }
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

            var result = await _target.AddTorrentAsync(new AddTorrentParams { Urls = new[] { "u" } }, cancellationToken: TestContext.Current.CancellationToken);

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
    }
}
