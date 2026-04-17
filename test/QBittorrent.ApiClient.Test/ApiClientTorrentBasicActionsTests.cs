using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientTorrentBasicActionsTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTorrentBasicActionsTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_LocationAndHashes_WHEN_SetTorrentLocation_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setLocation");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hashes=h1%7Ch2&location=%2Fdata%2Fdl");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentLocationAsync(TorrentSelector.FromHashes(["h1", "h2"]), "/data/dl", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetTorrentLocation_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.SetTorrentLocationAsync(TorrentSelector.AllTorrents(), "/x", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_NameAndHash_WHEN_SetTorrentName_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/rename");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hash=hx&name=My+Torrent");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentNameAsync("hx", "My Torrent", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetTorrentName_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("exists")
            });

            var result = await _target.SetTorrentNameAsync("h", "n", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "exists");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_SetTorrentComment_THEN_ShouldPOSTHashesAndComment()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.12.1");

                    case "/torrents/setComment":
                        req.Method.Should().Be(HttpMethod.Post);
                        (await req.Content.ReadAsStringOrNullAsync(ct)).Should().Be("hashes=h1%7Ch2&comment=Comment");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            (await _target.SetTorrentCommentAsync(TorrentSelector.FromHashes(["h1", "h2"]), "Comment", cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTorrentCommentEditing_WHEN_SetTorrentComment_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var commentRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.12.0"));

                    case "/torrents/setComment":
                        commentRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            var result = await _target.SetTorrentCommentAsync(TorrentSelector.FromHash("h1"), "Comment", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.12.0 does not support torrent comments.");
            commentRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_SetTorrentComment_THEN_ShouldThrowWhenClientIsNotInitialized()
        {
            var commentRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri?.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/setComment":
                        commentRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var action = async () => await _target.SetTorrentCommentAsync(TorrentSelector.FromHash("h1"), "Comment", cancellationToken: TestContext.Current.CancellationToken);

            await action.ShouldThrowUninitializedCompatibilityExceptionAsync();
            commentRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_SetTorrentSavePath_THEN_ShouldPOSTIdsAndPath()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setSavePath");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("id=a%7Cb&path=%2Fmnt%2Fsaves");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentSavePathAsync(TorrentSelector.FromHashes(["a", "b"]), "/mnt/saves", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ForbiddenWritableFailure_WHEN_SetTorrentSavePath_THEN_ShouldReturnDirectoryNotWritableFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("cannot write to directory")
            });

            var result = await _target.SetTorrentSavePathAsync(TorrentSelector.FromHash("a"), "/mnt/saves", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.AccessDenied,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "cannot write to directory");

            failure.TryGetReason<TorrentPathFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(TorrentPathFailureReason.DirectoryNotWritable);
        }

        [Fact]
        public async Task GIVEN_Conflict_WHEN_SetTorrentSavePath_THEN_ShouldReturnDirectoryCreationFailedFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("create failed")
            });

            var result = await _target.SetTorrentSavePathAsync(TorrentSelector.FromHash("a"), "/mnt/saves", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "create failed");

            failure.TryGetReason<TorrentPathFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(TorrentPathFailureReason.DirectoryCreationFailed);
        }

        [Fact]
        public async Task GIVEN_ConflictWithoutBody_WHEN_SetTorrentSavePath_THEN_ShouldUseDefaultDirectoryCreationFailedMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict));

            var result = await _target.SetTorrentSavePathAsync(TorrentSelector.FromHash("a"), "/mnt/saves", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "The target directory could not be created.");

            failure.TryGetReason<TorrentPathFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(TorrentPathFailureReason.DirectoryCreationFailed);
        }

        [Fact]
        public async Task GIVEN_ForbiddenWithoutWriteMessage_WHEN_SetTorrentSavePath_THEN_ShouldUseGenericForbiddenFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("permission denied")
            });

            var result = await _target.SetTorrentSavePathAsync(TorrentSelector.FromHash("a"), "/mnt/saves", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.AuthenticationRequired,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "permission denied");
        }

        [Fact]
        public async Task GIVEN_EmptyPath_WHEN_SetTorrentSavePath_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.SetTorrentSavePathAsync(TorrentSelector.FromHash("a"), string.Empty, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("path");
        }

        [Fact]
        public async Task GIVEN_EmptyHashes_WHEN_SetTorrentSavePath_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.SetTorrentSavePathAsync(TorrentSelector.FromHashes([]), "/path", cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_NullHashes_WHEN_SetTorrentSavePath_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.SetTorrentSavePathAsync(TorrentSelector.FromHashes(null!), "/path", cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_Hashes_WHEN_SetTorrentDownloadPath_THEN_ShouldPOSTIdsAndPath()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setDownloadPath");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("id=a%7Cb&path=temp");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHashes(["a", "b"]), "temp", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NullPath_WHEN_SetTorrentDownloadPath_THEN_ShouldPostEmptyPath()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setDownloadPath");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("id=a&path=");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHash("a"), null, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ForbiddenWritableFailure_WHEN_SetTorrentDownloadPath_THEN_ShouldReturnDirectoryNotWritableFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("cannot write to download path")
            });

            var result = await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHash("a"), "temp", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.AccessDenied,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "cannot write to download path");

            failure.TryGetReason<TorrentPathFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(TorrentPathFailureReason.DirectoryNotWritable);
        }

        [Fact]
        public async Task GIVEN_Conflict_WHEN_SetTorrentDownloadPath_THEN_ShouldReturnDirectoryCreationFailedFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("download create failed")
            });

            var result = await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHash("a"), "temp", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "download create failed");

            failure.TryGetReason<TorrentPathFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(TorrentPathFailureReason.DirectoryCreationFailed);
        }

        [Fact]
        public async Task GIVEN_ConflictWithoutBody_WHEN_SetTorrentDownloadPath_THEN_ShouldUseDefaultDirectoryCreationFailedMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict));

            var result = await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHash("a"), "temp", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "The target directory could not be created.");

            failure.TryGetReason<TorrentPathFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(TorrentPathFailureReason.DirectoryCreationFailed);
        }

        [Fact]
        public async Task GIVEN_ForbiddenWithoutWriteMessage_WHEN_SetTorrentDownloadPath_THEN_ShouldUseGenericForbiddenFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("permission denied")
            });

            var result = await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHash("a"), "temp", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.AuthenticationRequired,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "permission denied");
        }

        [Fact]
        public async Task GIVEN_EmptyHashes_WHEN_SetTorrentDownloadPath_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHashes([]), "temp", cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_NullHashes_WHEN_SetTorrentDownloadPath_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.SetTorrentDownloadPathAsync(TorrentSelector.FromHashes(null!), "temp", cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("hashes");
        }

        [Fact]
        public async Task GIVEN_CategoryAndHashes_WHEN_SetTorrentCategory_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setCategory");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hashes=h1%7Ch2&category=Movies");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetTorrentCategoryAsync(TorrentSelector.FromHashes(["h1", "h2"]), "Movies", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetTorrentCategory_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.SetTorrentCategoryAsync(TorrentSelector.AllTorrents(), "c", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadGateway, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_GetTorrentSslParameters_THEN_ShouldDeserialize()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ssl_certificate\":\"cert\",\"ssl_private_key\":\"key\",\"ssl_dh_params\":\"dh\"}")
            });

            var result = (await _target.GetTorrentSslParametersAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Certificate.Should().Be("cert");
            result.PrivateKey.Should().Be("key");
            result.DhParams.Should().Be("dh");
        }

        [Fact]
        public async Task GIVEN_Parameters_WHEN_SetTorrentSslParameters_THEN_ShouldPOSTAllFields()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/torrents/setSSLParameters");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("hash=abc&ssl_certificate=cert&ssl_private_key=key&ssl_dh_params=dh");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var parameters = new SslParameters("cert", "key", "dh");

            await _target.SetTorrentSslParametersAsync("abc", parameters, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NullParameters_WHEN_SetTorrentSslParameters_THEN_ShouldThrowArgumentNullException()
        {
            var action = async () => await _target.SetTorrentSslParametersAsync("abc", null!, cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("parameters");
        }

        [Fact]
        public async Task GIVEN_MissingRequiredSslFields_WHEN_SetTorrentSslParameters_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.SetTorrentSslParametersAsync("abc", new SslParameters("", "key", "dh"), cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("parameters");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetTorrentSslParameters_THEN_ShouldThrowHttpRequestException()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("ssl failed")
            });

            var result = await _target.SetTorrentSslParametersAsync("abc", new SslParameters("cert", "key", "dh"), cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadGateway, userMessage: "ssl failed");
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
