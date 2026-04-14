using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientApplicationTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientApplicationTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_OK_WHEN_GetApplicationVersion_THEN_ShouldReturnRawBody()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/version");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("4.6.0")
                };
            };

            var result = (await _target.GetApplicationVersionAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Be("4.6.0");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetApplicationVersion_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetApplicationVersionAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadGateway, userMessage: "bad");
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, ApiFailureKind.ValidationFailed, "The request was rejected.")]
        [InlineData(HttpStatusCode.Unauthorized, ApiFailureKind.AuthenticationRequired, "Authentication is required.")]
        [InlineData(HttpStatusCode.Forbidden, ApiFailureKind.AuthenticationRequired, "Authentication is required.")]
        [InlineData(HttpStatusCode.NotFound, ApiFailureKind.NotFound, "The requested resource could not be found.")]
        [InlineData(HttpStatusCode.Conflict, ApiFailureKind.Conflict, "The request conflicts with the current server state.")]
        [InlineData(HttpStatusCode.InternalServerError, ApiFailureKind.ServerError, "qBittorrent returned a server error.")]
        public async Task GIVEN_NonSuccessWithoutBody_WHEN_GetApplicationVersion_THEN_ShouldUseDefaultFailureMessage(
            HttpStatusCode statusCode,
            ApiFailureKind expectedKind,
            string expectedMessage)
        {
            _handler.Responder = (_, _) => Task.FromResult(CreateResponse(statusCode, null));

            var result = await _target.GetApplicationVersionAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: expectedKind,
                statusCode: statusCode,
                userMessage: expectedMessage);
        }

        [Fact]
        public async Task GIVEN_OK_WHEN_GetAPIVersion_THEN_ShouldReturnRawBody()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("2.10")
            });

            var result = (await _target.GetAPIVersionAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Be("2.10");
        }

        [Fact]
        public async Task GIVEN_CachedCompatibilityProfile_WHEN_GetAPIVersion_THEN_ShouldReturnCachedValueWithoutCallingEndpoint()
        {
            var apiVersionRequestCount = 0;

            _handler.Responder = (_, _) =>
            {
                apiVersionRequestCount++;
                return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.2"));
            };

            (await _target.RefreshCompatibilityAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            apiVersionRequestCount.Should().Be(1);

            var result = (await _target.GetAPIVersionAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Be("2.15.2");
            apiVersionRequestCount.Should().Be(1);
        }

        [Fact]
        public async Task GIVEN_GetAPIVersionHydratesCache_WHEN_LoadClientData_THEN_ShouldNotRequestApiVersionAgain()
        {
            var apiVersionRequestCount = 0;
            var loadRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        apiVersionRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.1"));

                    case "/clientdata/load":
                        loadRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.GetAPIVersionAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow().Should().Be("2.13.1");
            apiVersionRequestCount.Should().Be(1);

            (await _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            apiVersionRequestCount.Should().Be(1);
            loadRequestCount.Should().Be(1);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetAPIVersion_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("no")
            });

            var result = await _target.GetAPIVersionAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "no");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_RefreshCompatibilityAsync_THEN_ShouldSucceed()
        {
            var apiVersionRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/app/webapiVersion");
                apiVersionRequestCount++;
                return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.2"));
            };

            var result = await _target.RefreshCompatibilityAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldSucceed();
            apiVersionRequestCount.Should().Be(1);
        }

        [Fact]
        public async Task GIVEN_InvalidApiVersion_WHEN_RefreshCompatibilityAsync_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(CreateResponse(HttpStatusCode.OK, "invalid"));

            var result = await _target.RefreshCompatibilityAsync(cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "Unable to determine the qBittorrent Web API version.");

            failure.Detail.Should().Be("qBittorrent returned an unsupported Web API version value: invalid");
        }

        [Fact]
        public async Task GIVEN_EmptyApiVersion_WHEN_RefreshCompatibilityAsync_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(CreateResponse(HttpStatusCode.OK, null));

            var result = await _target.RefreshCompatibilityAsync(cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "Unable to determine the qBittorrent Web API version.");

            failure.Detail.Should().Be("qBittorrent did not return a Web API version.");
        }

        [Fact]
        public async Task GIVEN_ApiVersionEndpointFailure_WHEN_RefreshCompatibilityAsync_THEN_ShouldReturnProbeFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

            var result = await _target.RefreshCompatibilityAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");
        }

        [Fact]
        public async Task GIVEN_UnknownStatus_WHEN_Shutdown_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(CreateResponse((HttpStatusCode)418, "teapot"));

            var result = await _target.ShutdownAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                statusCode: (HttpStatusCode)418,
                userMessage: "teapot");
        }

        [Fact]
        public async Task GIVEN_UnsupportedMediaType_WHEN_Shutdown_THEN_ShouldReturnUnsupportedData()
        {
            _handler.Responder = (_, _) => Task.FromResult(CreateResponse(HttpStatusCode.UnsupportedMediaType, "unsupported"));

            var result = await _target.ShutdownAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnsupportedData,
                statusCode: HttpStatusCode.UnsupportedMediaType,
                userMessage: "unsupported");
        }

        [Fact]
        public async Task GIVEN_UnsupportedMediaTypeWithoutBody_WHEN_Shutdown_THEN_ShouldUseDefaultUnsupportedDataMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(CreateResponse(HttpStatusCode.UnsupportedMediaType, null));

            var result = await _target.ShutdownAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnsupportedData,
                statusCode: HttpStatusCode.UnsupportedMediaType,
                userMessage: "The supplied data is not supported.");
        }

        [Fact]
        public async Task GIVEN_TaskCanceledException_WHEN_GetApplicationVersion_THEN_ShouldReturnTimeout()
        {
            _handler.Responder = (_, _) => throw new TaskCanceledException("timed out");

            var result = await _target.GetApplicationVersionAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.Timeout,
                userMessage: "The request timed out before qBittorrent responded.");
        }

        [Fact]
        public async Task GIVEN_CanceledToken_WHEN_GetApplicationVersion_THEN_ShouldThrowOperationCanceledException()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _handler.Responder = (_, cancellationToken) => Task.FromCanceled<HttpResponseMessage>(cancellationToken);

            var action = async () => await _target.GetApplicationVersionAsync(cancellationTokenSource.Token);

            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task GIVEN_EmptyHttpRequestExceptionMessage_WHEN_GetApplicationVersion_THEN_ShouldUseDefaultNoResponseMessage()
        {
            _handler.Responder = (_, _) => throw new HttpRequestException(string.Empty);

            var result = await _target.GetApplicationVersionAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.NoResponse,
                userMessage: "qBittorrent client is not reachable.");
        }

        [Fact]
        public async Task GIVEN_MissingBaseAddress_WHEN_GetApplicationVersion_THEN_ShouldReturnConfigurationFailure()
        {
            var target = new ApiClient(new HttpClient(_handler));

            var result = await target.GetApplicationVersionAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.InvalidConfiguration,
                userMessage: "HttpClient BaseAddress must be configured.");
        }

        [Fact]
        public async Task GIVEN_MissingBaseAddress_WHEN_GetAPIVersion_THEN_ShouldReturnConfigurationFailure()
        {
            var target = new ApiClient(new HttpClient(_handler));

            var result = await target.GetAPIVersionAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.InvalidConfiguration,
                userMessage: "HttpClient BaseAddress must be configured.");
        }

        [Fact]
        public async Task GIVEN_InvalidApiVersionResponse_WHEN_GetAPIVersionThenLoadClientData_THEN_ShouldNotCacheInvalidCompatibilityProfile()
        {
            var apiVersionRequestCount = 0;
            var loadRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        apiVersionRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, apiVersionRequestCount == 1 ? "invalid" : "2.13.1"));

                    case "/clientdata/load":
                        loadRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.GetAPIVersionAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow().Should().Be("invalid");
            (await _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            apiVersionRequestCount.Should().Be(2);
            loadRequestCount.Should().Be(1);
        }

        [Fact]
        public async Task GIVEN_EmptyApiVersionResponse_WHEN_GetAPIVersionThenLoadClientData_THEN_ShouldNotCacheCompatibilityProfile()
        {
            var apiVersionRequestCount = 0;
            var loadRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        apiVersionRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, apiVersionRequestCount == 1 ? null : "2.13.1"));

                    case "/clientdata/load":
                        loadRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.GetAPIVersionAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow().Should().Be(string.Empty);
            (await _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            apiVersionRequestCount.Should().Be(2);
            loadRequestCount.Should().Be(1);
        }

        [Fact]
        public async Task GIVEN_LoadClientDataCalls_WHEN_RefreshCompatibilityAsyncIsCalled_THEN_ShouldClearAndReloadCompatibilityProfile()
        {
            var apiVersionRequestCount = 0;
            var loadRequestCount = 0;

            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        apiVersionRequestCount++;
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/load":
                        loadRequestCount++;
                        return CreateResponse(HttpStatusCode.OK, "{}");

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            (await _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            apiVersionRequestCount.Should().Be(1);
            loadRequestCount.Should().Be(2);

            (await _target.RefreshCompatibilityAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            apiVersionRequestCount.Should().Be(2);

            (await _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            apiVersionRequestCount.Should().Be(2);
            loadRequestCount.Should().Be(3);
        }

        [Fact]
        public async Task GIVEN_ConcurrentCompatibilityRequests_WHEN_LoadClientData_THEN_ShouldResolveVersionOnce()
        {
            var apiVersionRequestCount = 0;
            var loadRequestCount = 0;
            var releaseVersionResponse = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        apiVersionRequestCount++;
                        await releaseVersionResponse.Task.WaitAsync(ct);
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/load":
                        loadRequestCount++;
                        return CreateResponse(HttpStatusCode.OK, "{}");

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var firstTask = _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken);
            var secondTask = _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken);

            await Task.Yield();
            apiVersionRequestCount.Should().Be(1);

            releaseVersionResponse.SetResult();

            var results = await Task.WhenAll(firstTask, secondTask);

            results.Should().AllSatisfy(result => result.ShouldSucceed());
            apiVersionRequestCount.Should().Be(1);
            loadRequestCount.Should().Be(2);
        }

        [Fact]
        public async Task GIVEN_SharedCompatibilityCacheAndDifferentBaseAddresses_WHEN_LoadClientData_THEN_ShouldResolveVersionPerBaseAddress()
        {
            var compatibilityProfileCache = new ApiClientCompatibilityProfileCache();
            var firstHandler = new StubHttpMessageHandler();
            var secondHandler = new StubHttpMessageHandler();
            var firstApiVersionRequestCount = 0;
            var secondApiVersionRequestCount = 0;

            var firstClient = new ApiClient(
                new HttpClient(firstHandler)
                {
                    BaseAddress = new Uri("http://localhost-a/api/v2/")
                },
                compatibilityProfileCache);

            var secondClient = new ApiClient(
                new HttpClient(secondHandler)
                {
                    BaseAddress = new Uri("http://localhost-b/api/v2/")
                },
                compatibilityProfileCache);

            firstHandler.Responder = (request, _) =>
            {
                return request.RequestUri!.AbsolutePath switch
                {
                    "/api/v2/app/webapiVersion" => Task.FromResult(CreateResponse(HttpStatusCode.OK, (++firstApiVersionRequestCount, "2.13.1").Item2)),
                    "/api/v2/clientdata/load" => Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}")),
                    _ => throw new InvalidOperationException($"Unexpected request: {request.RequestUri}")
                };
            };

            secondHandler.Responder = (request, _) =>
            {
                return request.RequestUri!.AbsolutePath switch
                {
                    "/api/v2/app/webapiVersion" => Task.FromResult(CreateResponse(HttpStatusCode.OK, (++secondApiVersionRequestCount, "2.15.2").Item2)),
                    "/api/v2/clientdata/load" => Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}")),
                    _ => throw new InvalidOperationException($"Unexpected request: {request.RequestUri}")
                };
            };

            (await firstClient.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            (await secondClient.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();

            firstApiVersionRequestCount.Should().Be(1);
            secondApiVersionRequestCount.Should().Be(1);
        }

        [Fact]
        public async Task GIVEN_KeysAndSupportedApiVersion_WHEN_LoadClientData_THEN_ShouldPostKeysAndReturnEntries()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        req.Method.Should().Be(HttpMethod.Get);
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/load":
                        req.Method.Should().Be(HttpMethod.Post);
                        var body = await req.Content!.ReadAsStringAsync(ct);
                        body.Should().Contain("keys=");
                        return CreateResponse(
                            HttpStatusCode.OK,
                            """
                            {
                                "QbtMud.AppSettings.State.v1": {"theme":"dark"},
                                "QbtMud.WebUiLocalization.PreferredLocale.v1": "en"
                            }
                            """);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.LoadClientDataAsync(["QbtMud.AppSettings.State.v1", "QbtMud.WebUiLocalization.PreferredLocale.v1"], cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().HaveCount(2);
            result["QbtMud.AppSettings.State.v1"].GetProperty("theme").GetString().Should().Be("dark");
            result["QbtMud.WebUiLocalization.PreferredLocale.v1"].GetString().Should().Be("en");
        }

        [Fact]
        public async Task GIVEN_NoKeysAndSupportedApiVersion_WHEN_LoadClientData_THEN_ShouldPostAndReturnEntries()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/load":
                        req.Method.Should().Be(HttpMethod.Post);
                        var body = await req.Content!.ReadAsStringAsync(ct);
                        body.Should().BeEmpty();
                        return CreateResponse(
                            HttpStatusCode.OK,
                            """
                            {
                                "QbtMud.AppSettings.State.v1": {"value":true}
                            }
                            """);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainKey("QbtMud.AppSettings.State.v1");
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeClientData_WHEN_LoadClientData_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var loadRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.0"));

                    case "/clientdata/load":
                        loadRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.LoadClientDataAsync(["QbtMud.Test"], cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.13.0 does not support the client data API.");
            loadRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndEndpointFailure_WHEN_LoadClientData_THEN_ShouldReturnEndpointFailure()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.1"));

                    case "/clientdata/load":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadRequest, "load failed"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.LoadClientDataAsync(["QbtMud.Test"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "load failed");
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_LoadClientData_THEN_ShouldReturnProbeFailure()
        {
            var loadRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/clientdata/load":
                        loadRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.LoadClientDataAsync(["QbtMud.Test"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");

            loadRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_DataAndSupportedApiVersion_WHEN_StoreClientData_THEN_ShouldPostDataPayload()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/store":
                        req.Method.Should().Be(HttpMethod.Post);
                        var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                        body.Should().Contain("data=");
                        body.Should().Contain("\"QbtMud.AppSettings.State.v1\":{\"notifications\":true}");
                        body.Should().Contain("\"QbtMud.Search.Jobs\":null");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            await _target.StoreClientDataAsync(new Dictionary<string, JsonElement?>
            {
                ["QbtMud.AppSettings.State.v1"] = CreateJsonElement(new { notifications = true }),
                ["QbtMud.Search.Jobs"] = null
            }, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeClientData_WHEN_StoreClientData_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var storeRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.0"));

                    case "/clientdata/store":
                        storeRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.StoreClientDataAsync(new Dictionary<string, JsonElement?>
            {
                ["QbtMud.AppSettings.State.v1"] = CreateJsonElement(new { value = true })
            }, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.13.0 does not support the client data API.");
            storeRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndEndpointFailure_WHEN_StoreClientData_THEN_ShouldReturnEndpointFailure()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.13.1"));

                    case "/clientdata/store":
                        return Task.FromResult(CreateResponse(HttpStatusCode.Conflict, "store failed"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.StoreClientDataAsync(new Dictionary<string, JsonElement?>
            {
                ["QbtMud.AppSettings.State.v1"] = CreateJsonElement(new { value = true })
            }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "store failed");
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_StoreClientData_THEN_ShouldReturnProbeFailure()
        {
            var storeRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/clientdata/store":
                        storeRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, null));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.StoreClientDataAsync(new Dictionary<string, JsonElement?>
            {
                ["QbtMud.AppSettings.State.v1"] = CreateJsonElement(new { value = true })
            }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");

            storeRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_JsonNullValue_WHEN_StoreClientData_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.StoreClientDataAsync(new Dictionary<string, JsonElement?>
            {
                ["QbtMud.AppSettings.State.v1"] = CreateNullJsonElement()
            }, cancellationToken: TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("data");
        }

        [Fact]
        public async Task GIVEN_UndefinedJsonValue_WHEN_StoreClientData_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.StoreClientDataAsync(new Dictionary<string, JsonElement?>
            {
                ["QbtMud.AppSettings.State.v1"] = default(JsonElement)
            }, cancellationToken: TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("data");
        }

        [Fact]
        public async Task GIVEN_Data_WHEN_UpsertClientData_THEN_ShouldDelegateToStorePayloadWithoutDeletes()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/store":
                        var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                        body.Should().Contain("\"QbtMud.AppSettings.State.v1\":{\"notifications\":true}");
                        body.Should().NotContain(":null");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.UpsertClientDataAsync(new Dictionary<string, JsonElement>
            {
                ["QbtMud.AppSettings.State.v1"] = CreateJsonElement(new { notifications = true })
            }, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_JsonNullValue_WHEN_UpsertClientData_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.UpsertClientDataAsync(new Dictionary<string, JsonElement>
            {
                ["QbtMud.AppSettings.State.v1"] = CreateNullJsonElement()
            }, cancellationToken: TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("data");
        }

        [Fact]
        public async Task GIVEN_UndefinedJsonValue_WHEN_UpsertClientData_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.UpsertClientDataAsync(new Dictionary<string, JsonElement>
            {
                ["QbtMud.AppSettings.State.v1"] = default
            }, cancellationToken: TestContext.Current.CancellationToken);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("data");
        }

        [Fact]
        public async Task GIVEN_Keys_WHEN_DeleteClientData_THEN_ShouldDelegateToStorePayloadWithDeletes()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/store":
                        var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                        body.Should().Contain("\"QbtMud.AppSettings.State.v1\":null");
                        body.Should().Contain("\"QbtMud.Search.Jobs\":null");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.DeleteClientDataAsync(
                ["QbtMud.AppSettings.State.v1", " QbtMud.Search.Jobs ", "QbtMud.Search.Jobs"],
                cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_EmptyKeys_WHEN_DeleteClientData_THEN_ShouldPostEmptyPatch()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.13.1");

                    case "/clientdata/store":
                        var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                        body.Should().Contain("data={}");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.DeleteClientDataAsync([], cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_OKAndJson_WHEN_GetBuildInfo_THEN_ShouldDeserialize()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            var result = (await _target.GetBuildInfoAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_RichBuildInfoJson_WHEN_GetBuildInfo_THEN_ShouldMapAllFields()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                        "qt": "QtVersion",
                        "libtorrent": "LibTorrentVersion",
                        "boost": "BoostVersion",
                        "openssl": "OpenSslVersion",
                        "zlib": "ZLibVersion",
                        "bitness": 64,
                        "platform": "windows"
                    }
                    """)
            });

            var result = (await _target.GetBuildInfoAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.QTVersion.Should().Be("QtVersion");
            result.LibTorrentVersion.Should().Be("LibTorrentVersion");
            result.BoostVersion.Should().Be("BoostVersion");
            result.OpenSSLVersion.Should().Be("OpenSslVersion");
            result.ZLibVersion.Should().Be("ZLibVersion");
            result.Bitness.Should().Be(64);
            result.Platform.Should().Be(BuildPlatform.Windows);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetBuildInfo_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.GetBuildInfoAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_GetProcessInfo_THEN_ShouldDeserialize()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.1"));

                    case "/app/processInfo":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, """{"launch_time":12345}"""));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.GetProcessInfoAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.LaunchTime.Should().Be(12345);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeProcessInfo_WHEN_GetProcessInfo_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var processInfoRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.0"));

                    case "/app/processInfo":
                        processInfoRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, """{"launch_time":1}"""));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetProcessInfoAsync(cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.15.0 does not support process info.");
            processInfoRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_GetProcessInfo_THEN_ShouldReturnProbeFailure()
        {
            var processInfoRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/app/processInfo":
                        processInfoRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, """{"launch_time":1}"""));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetProcessInfoAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            processInfoRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_OK_WHEN_Shutdown_THEN_ShouldPostAndNotThrow()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/shutdown");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            };

            await _target.ShutdownAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_Shutdown_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("busy")
            });

            var result = await _target.ShutdownAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.ServiceUnavailable, userMessage: "busy");
        }

        [Fact]
        public async Task GIVEN_OKAndJson_WHEN_GetApplicationPreferences_THEN_ShouldDeserialize()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            var result = (await _target.GetApplicationPreferencesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            var serialized = JsonSerializer.Serialize(result);
            serialized.Should().Contain("\"up_limit\":0");
        }

        [Fact]
        public async Task GIVEN_PreferencesWithScanDirs_WHEN_GetApplicationPreferences_THEN_ShouldMapSaveLocations()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                        "up_limit": 10240,
                        "scan_dirs":
                        {
                            "Watch": 0,
                            "Default": 1,
                            "Custom": "/downloads/custom"
                        }
                    }
                    """)
            });

            var result = (await _target.GetApplicationPreferencesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.UpLimit.Should().Be(10240);
            result.ScanDirs.Should().HaveCount(3);
            result.ScanDirs["Watch"].Kind.Should().Be(SaveLocationKind.WatchedFolder);
            result.ScanDirs["Default"].Kind.Should().Be(SaveLocationKind.DefaultFolder);
            result.ScanDirs["Custom"].Kind.Should().Be(SaveLocationKind.CustomPath);
            result.ScanDirs["Custom"].SavePath.Should().Be("/downloads/custom");
        }

        [Fact]
        public async Task GIVEN_PreferencesWithEnumBackedFields_WHEN_GetApplicationPreferences_THEN_ShouldMapEnumValues()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                        "up_limit": 10240,
                        "auto_delete_mode": 2,
                        "bittorrent_protocol": 1,
                        "disk_io_read_mode": 0,
                        "disk_io_type": 3,
                        "disk_io_write_mode": 1,
                        "dyndns_service": -1,
                        "encryption": 2,
                        "hostname_cache_ttl": 300,
                        "lsd": true,
                        "max_ratio_act": 3,
                        "pex": true,
                        "proxy_type": "SOCKS5",
                        "resolve_peer_host_names": true,
                        "resume_data_storage_type": "SQLite",
                        "scheduler_days": 2,
                        "torrent_content_layout": "NoSubfolder",
                        "torrent_content_remove_option": "MoveToTrash",
                        "torrent_stop_condition": "FilesChecked",
                        "upload_slots_behavior": 1,
                        "upload_choking_algorithm": 2,
                        "use_subcategories": true,
                        "utp_tcp_mixed_mode": 1
                    }
                    """)
            });

            var result = (await _target.GetApplicationPreferencesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.AutoDeleteMode.Should().Be(AutoDeleteMode.Always);
            result.BittorrentProtocol.Should().Be(BittorrentProtocol.TcpOnly);
            result.DiskIoReadMode.Should().Be(DiskIoReadMode.DisableOsCache);
            result.DiskIoType.Should().Be(DiskIoType.SimplePreadPwrite);
            result.DiskIoWriteMode.Should().Be(DiskIoWriteMode.EnableOsCache);
            result.DyndnsService.Should().Be(DyndnsService.None);
            result.Encryption.Should().Be(EncryptionMode.DisableEncryption);
            result.HostnameCacheTtl.Should().Be(300);
            result.Lsd.Should().BeTrue();
            result.MaxRatioAct.Should().Be(MaxRatioAction.RemoveTorrentAndFiles);
            result.Pex.Should().BeTrue();
            result.ProxyType.Should().Be(ProxyType.Socks5);
            result.ResolvePeerHostNames.Should().BeTrue();
            result.ResumeDataStorageType.Should().Be(ResumeDataStorageType.Sqlite);
            result.SchedulerDays.Should().Be(SchedulerDays.Weekends);
            result.TorrentContentLayout.Should().Be(TorrentContentLayout.NoSubfolder);
            result.TorrentContentRemoveOption.Should().Be(TorrentContentRemoveOption.MoveToTrash);
            result.TorrentStopCondition.Should().Be(StopCondition.FilesChecked);
            result.UploadSlotsBehavior.Should().Be(UploadSlotsBehavior.UploadRateBased);
            result.UploadChokingAlgorithm.Should().Be(UploadChokingAlgorithm.AntiLeech);
            result.UseSubcategories.Should().BeTrue();
            result.UtpTcpMixedMode.Should().Be(UtpTcpMixedMode.PeerProportional);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetApplicationPreferences_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad prefs")
            });

            var result = await _target.GetApplicationPreferencesAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad prefs");
        }

        [Fact]
        public async Task GIVEN_Preferences_WHEN_SetApplicationPreferences_THEN_ShouldPostJsonFormAndSucceed()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/setPreferences");
                req.Content!.Headers.ContentType!.MediaType.Should().Be("application/x-www-form-urlencoded");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().StartWith("json=");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var prefs = new UpdatePreferences();
            await _target.SetApplicationPreferencesAsync(prefs, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_MaxRatioPreference_WHEN_SetApplicationPreferences_THEN_ShouldSerializeMaxRatioAsJsonNumber()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/setPreferences");

                var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                body.Should().StartWith("json=");

                var json = body["json=".Length..];
                json.Should().Contain("\"max_ratio\":1.23456789012345");
                json.Should().NotContain("\"max_ratio\":\"1.23456789012345\"");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var preferences = new UpdatePreferences
            {
                MaxRatio = 1.23456789012345
            };

            (await _target.SetApplicationPreferencesAsync(preferences, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_EnumBackedPreferences_WHEN_SetApplicationPreferences_THEN_ShouldSerializeNumericAndStringEnumValues()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/setPreferences");

                var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                body.Should().StartWith("json=");

                var json = body["json=".Length..];
                json.Should().Contain("\"auto_delete_mode\":1");
                json.Should().Contain("\"bittorrent_protocol\":2");
                json.Should().Contain("\"disk_io_read_mode\":1");
                json.Should().Contain("\"disk_io_type\":2");
                json.Should().Contain("\"disk_io_write_mode\":2");
                json.Should().Contain("\"dyndns_service\":-1");
                json.Should().Contain("\"encryption\":1");
                json.Should().Contain("\"hostname_cache_ttl\":300");
                json.Should().Contain("\"lsd\":true");
                json.Should().Contain("\"max_ratio_act\":2");
                json.Should().Contain("\"pex\":true");
                json.Should().Contain("\"proxy_type\":\"HTTP\"");
                json.Should().Contain("\"resolve_peer_host_names\":true");
                json.Should().Contain("\"resume_data_storage_type\":\"Legacy\"");
                json.Should().Contain("\"scheduler_days\":7");
                json.Should().Contain("\"torrent_content_layout\":\"Subfolder\"");
                json.Should().Contain("\"torrent_content_remove_option\":\"Delete\"");
                json.Should().Contain("\"torrent_stop_condition\":\"MetadataReceived\"");
                json.Should().Contain("\"upload_slots_behavior\":0");
                json.Should().Contain("\"upload_choking_algorithm\":1");
                json.Should().Contain("\"utp_tcp_mixed_mode\":0");
                json.Should().NotContain("\"auto_delete_mode\":\"1\"");
                json.Should().NotContain("\"bittorrent_protocol\":\"2\"");
                json.Should().NotContain("\"proxy_type\":1");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var preferences = new UpdatePreferences
            {
                AutoDeleteMode = AutoDeleteMode.IfAdded,
                BittorrentProtocol = BittorrentProtocol.UtpOnly,
                DiskIoReadMode = DiskIoReadMode.EnableOsCache,
                DiskIoType = DiskIoType.PosixCompliant,
                DiskIoWriteMode = DiskIoWriteMode.WriteThrough,
                DyndnsService = DyndnsService.None,
                Encryption = EncryptionMode.RequireEncryption,
                HostnameCacheTtl = 300,
                Lsd = true,
                MaxRatioAct = MaxRatioAction.EnableSuperSeeding,
                Pex = true,
                ProxyType = ProxyType.Http,
                ResolvePeerHostNames = true,
                ResumeDataStorageType = ResumeDataStorageType.Legacy,
                SchedulerDays = SchedulerDays.Friday,
                TorrentContentLayout = TorrentContentLayout.Subfolder,
                TorrentContentRemoveOption = TorrentContentRemoveOption.Delete,
                TorrentStopCondition = StopCondition.MetadataReceived,
                UploadSlotsBehavior = UploadSlotsBehavior.FixedSlots,
                UploadChokingAlgorithm = UploadChokingAlgorithm.FastestUpload,
                UtpTcpMixedMode = UtpTcpMixedMode.PreferTcp
            };

            (await _target.SetApplicationPreferencesAsync(preferences, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetApplicationPreferences_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("conflict")
            });

            var prefs = new UpdatePreferences();
            var result = await _target.SetApplicationPreferencesAsync(prefs, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "conflict");
        }

        [Fact]
        public async Task GIVEN_OKAndJsonList_WHEN_GetApplicationCookies_THEN_ShouldReturnUnexpectedResponseOnBadJson()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not json")
            });

            var result = await _target.GetApplicationCookiesAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_CookiesJson_WHEN_GetApplicationCookies_THEN_ShouldDeserializeCookies()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    [
                        {
                            "name": "Name",
                            "domain": "Domain",
                            "path": "/Path",
                            "value": "Value",
                            "expirationDate": 1700000000
                        }
                    ]
                    """)
            });

            var result = (await _target.GetApplicationCookiesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            result[0].Name.Should().Be("Name");
            result[0].Domain.Should().Be("Domain");
            result[0].Path.Should().Be("/Path");
            result[0].Value.Should().Be("Value");
            result[0].ExpirationDate.Should().Be(1700000000);
        }

        [Fact]
        public async Task GIVEN_ListOfCookies_WHEN_SetApplicationCookies_THEN_ShouldPostJsonArrayInForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/setCookies");
                req.Content!.Headers.ContentType!.MediaType.Should().Be("application/x-www-form-urlencoded");
                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().StartWith("cookies=");
                body.Should().Contain("%5B"); // '[' encoded
                body.Should().Contain("%5D"); // ']' encoded
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var cookies = new List<ApplicationCookie>();
            await _target.SetApplicationCookiesAsync(cookies, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_RotateApiKey_THEN_ShouldReturnNewKey()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.14.1"));

                    case "/app/rotateAPIKey":
                        req.Method.Should().Be(HttpMethod.Post);
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, """{"apiKey":"ApiKey"}"""));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.RotateAPIKeyAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Key.Should().Be("ApiKey");
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeApiKeyManagement_WHEN_RotateApiKey_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var rotateRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.14.0"));

                    case "/app/rotateAPIKey":
                        rotateRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, """{"apiKey":"ApiKey"}"""));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.RotateAPIKeyAsync(cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.14.0 does not support Web API key rotation.");
            rotateRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_RotateApiKey_THEN_ShouldReturnProbeFailure()
        {
            var rotateRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/app/rotateAPIKey":
                        rotateRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, """{"apiKey":"ApiKey"}"""));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.RotateAPIKeyAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            rotateRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_DeleteApiKey_THEN_ShouldPostAndSucceed()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.14.1"));

                    case "/app/deleteAPIKey":
                        req.Method.Should().Be(HttpMethod.Post);
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.DeleteAPIKeyAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeApiKeyManagement_WHEN_DeleteApiKey_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var deleteRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.14.0"));

                    case "/app/deleteAPIKey":
                        deleteRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.DeleteAPIKeyAsync(cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.14.0 does not support Web API key deletion.");
            deleteRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_DeleteApiKey_THEN_ShouldReturnProbeFailure()
        {
            var deleteRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/app/deleteAPIKey":
                        deleteRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.DeleteAPIKeyAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            deleteRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_Success_WHEN_SendTestEmail_THEN_ShouldPOSTToEndpoint()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/sendTestEmail");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            };

            await _target.SendTestEmailAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Parameters_WHEN_GetDirectoryContent_THEN_ShouldQueryAndReturnList()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/getDirectoryContent?dirPath=%2Fdata&mode=dirs");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[\"/data/folder\"]")
                });
            };

            var result = (await _target.GetDirectoryContentAsync("/data", DirectoryContentMode.Directories, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle().Which.Should().Be("/data/folder");
        }

        [Fact]
        public async Task GIVEN_FilesMode_WHEN_GetDirectoryContent_THEN_ShouldUseFilesModeQuery()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/getDirectoryContent?dirPath=%2Fdata&mode=files");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[\"/data/file1\"]")
                });
            };

            var result = (await _target.GetDirectoryContentAsync("/data", DirectoryContentMode.Files, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle().Which.Should().Be("/data/file1");
        }

        [Fact]
        public async Task GIVEN_DefaultMode_WHEN_GetDirectoryContent_THEN_ShouldUseAllModeQuery()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/getDirectoryContent?dirPath=%2Fdata&mode=all");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var result = (await _target.GetDirectoryContentAsync("/data", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_WhitespacePath_WHEN_GetDirectoryContent_THEN_ShouldThrowArgumentException()
        {
            var action = async () => await _target.GetDirectoryContentAsync(" ", cancellationToken: TestContext.Current.CancellationToken);

            var exception = await action.Should().ThrowAsync<ArgumentException>();
            exception.Which.ParamName.Should().Be("directoryPath");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetDirectoryContent_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad directory")
            });

            var result = await _target.GetDirectoryContentAsync("/data", DirectoryContentMode.Directories, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad directory");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_GetDirectoryContentEntries_THEN_ShouldQueryWithMetadataAndReturnEntries()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/app/getDirectoryContent":
                        req.Method.Should().Be(HttpMethod.Get);
                        req.RequestUri!.ToString().Should().Be("http://localhost/app/getDirectoryContent?dirPath=%2Fdata&mode=files&withMetadata=true");
                        return Task.FromResult(CreateResponse(
                            HttpStatusCode.OK,
                            """
                            [
                                {
                                    "name": "file.iso",
                                    "type": "file",
                                    "size": 42,
                                    "creation_date": 100,
                                    "last_access_date": 101,
                                    "last_modification_date": 102
                                }
                            ]
                            """));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.GetDirectoryContentEntriesAsync("/data", DirectoryContentMode.Files, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            result[0].Name.Should().Be("file.iso");
            result[0].Type.Should().Be(DirectoryContentEntryType.File);
            result[0].Size.Should().Be(42);
            result[0].CreationDate.Should().Be(100);
            result[0].LastAccessDate.Should().Be(101);
            result[0].LastModificationDate.Should().Be(102);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeDirectoryContentMetadata_WHEN_GetDirectoryContentEntries_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var directoryRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.7"));

                    case "/app/getDirectoryContent":
                        directoryRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "[]"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetDirectoryContentEntriesAsync("/data", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.7 does not support directory metadata responses.");
            directoryRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_GetDirectoryContentEntries_THEN_ShouldReturnProbeFailure()
        {
            var directoryRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/app/getDirectoryContent":
                        directoryRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "[]"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetDirectoryContentEntriesAsync("/data", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            directoryRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_OK_WHEN_GetDefaultSavePath_THEN_ShouldReturnRawBody()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("/data/downloads")
            });

            var result = (await _target.GetDefaultSavePathAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Be("/data/downloads");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetDefaultSavePath_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("nope")
            });

            var result = await _target.GetDefaultSavePathAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "nope");
        }

        [Fact]
        public async Task GIVEN_BadJson_WHEN_GetNetworkInterfaces_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not json")
            });

            var result = await _target.GetNetworkInterfacesAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NetworkInterfacesJson_WHEN_GetNetworkInterfaces_THEN_ShouldDeserializeInterfaces()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    [
                        {
                            "name": "eth0",
                            "value": "Ethernet 0"
                        }
                    ]
                    """)
            });

            var result = (await _target.GetNetworkInterfacesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            result[0].Name.Should().Be("eth0");
            result[0].Value.Should().Be("Ethernet 0");
        }

        [Fact]
        public async Task GIVEN_OKJsonArrayOfStrings_WHEN_GetNetworkInterfaceAddressList_THEN_ShouldDeserializeStrings()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/app/networkInterfaceAddressList?iface=eth0");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[\"192.168.1.10\",\"fe80::1\"]")
                });
            };

            var result = (await _target.GetNetworkInterfaceAddressListAsync("eth0", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Should().Be("192.168.1.10");
            result[1].Should().Be("fe80::1");
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

        private static JsonElement CreateJsonElement<T>(T value)
        {
            return JsonSerializer.SerializeToElement(value);
        }

        private static JsonElement CreateNullJsonElement()
        {
            return JsonDocument.Parse("null").RootElement.Clone();
        }
    }
}
