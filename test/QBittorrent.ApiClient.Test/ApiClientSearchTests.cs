using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientSearchTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientSearchTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_PatternAndPlugins_WHEN_StartSearch_THEN_ShouldPOSTFormAndReturnId()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/start");
                req.Content!.Headers.ContentType!.MediaType.Should().Be("application/x-www-form-urlencoded");

                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("pattern=My+pattern&plugins=a%7Cb%7Cc&category=all");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\":123}")
                };
            };

            var id = (await _target.StartSearchAsync("My pattern", ["a", "b", "c"], cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be(123);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_StartSearch_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.StartSearchAsync("p", ["x"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_ResponseWithoutId_WHEN_StartSearch_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"name\":\"search\"}")
            });

            var result = await _target.StartSearchAsync("p", ["x"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_ResponseWithNonIntegerId_WHEN_StartSearch_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":\"search\"}")
            });

            var result = await _target.StartSearchAsync("p", ["x"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_Id_WHEN_StopSearch_THEN_ShouldPOSTFormWithId()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/stop");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("id=77");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.StopSearchAsync(77, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Id_WHEN_GetSearchStatus_THEN_ShouldGETWithIdAndReturnNotFoundOnEmpty()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/status?id=5");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var result = await _target.GetSearchStatusAsync(5, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.NotFound, userMessage: "The search job could not be found.");
        }

        [Fact]
        public async Task GIVEN_SearchStatusPayload_WHEN_GetSearchStatus_THEN_ShouldDeserializeStatus()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/status?id=5");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "id": 5,
                                "status": "Running",
                                "total": 12
                            }
                        ]
                        """)
                });
            };

            var status = (await _target.GetSearchStatusAsync(5, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            status.Id.Should().Be(5);
            status.Status.Should().Be(SearchJobStatus.Running);
            status.Total.Should().Be(12);
        }

        [Fact]
        public async Task GIVEN_NotFound_WHEN_GetSearchStatus_THEN_ShouldReturnFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));

            var result = await _target.GetSearchStatusAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.NotFound, userMessage: "The search job could not be found.");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetSearchStatus_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("nope")
            });

            var result = await _target.GetSearchStatusAsync(2, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "nope");
        }

        [Fact]
        public async Task GIVEN_Request_WHEN_GetSearchesStatus_THEN_ShouldGETAndReturnUnexpectedResponseOnBadJson()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            });

            var list = (await _target.GetSearchesStatusAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();
            list.Should().NotBeNull();
            list.Count.Should().Be(0);

            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("oops")
            });

            var result = await _target.GetSearchesStatusAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_SearchStatusesPayload_WHEN_GetSearchesStatus_THEN_ShouldDeserializeList()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    [
                        {
                            "id": 1,
                            "status": "Running",
                            "total": 4
                        },
                        {
                            "id": 2,
                            "status": "Stopped",
                            "total": 5
                        }
                    ]
                    """)
            });

            var list = (await _target.GetSearchesStatusAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            list.Should().HaveCount(2);
            list[0].Id.Should().Be(1);
            list[0].Status.Should().Be(SearchJobStatus.Running);
            list[0].Total.Should().Be(4);
            list[1].Id.Should().Be(2);
            list[1].Status.Should().Be(SearchJobStatus.Stopped);
            list[1].Total.Should().Be(5);
        }

        [Fact]
        public async Task GIVEN_IdOnly_WHEN_GetSearchResults_THEN_ShouldGETWithIdOnly()
        {
            _handler.Responder = (req, _) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/search/results?id=9");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            };

            var results = (await _target.GetSearchResultsAsync(9, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            results.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_SearchResultsPayload_WHEN_GetSearchResults_THEN_ShouldDeserializeResults()
        {
            _handler.Responder = (req, _) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/search/results?id=9");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "results":
                            [
                                {
                                    "descrLink": "https://example.com/details",
                                    "fileName": "FileName",
                                    "fileSize": 12345,
                                    "fileUrl": "magnet:?xt=urn:btih:hash",
                                    "nbLeechers": 6,
                                    "nbSeeders": 7,
                                    "siteUrl": "https://example.com",
                                    "engineName": "Engine",
                                    "pubDate": 1700000000
                                }
                            ],
                            "status": "Running",
                            "total": 1
                        }
                        """)
                });
            };

            var results = (await _target.GetSearchResultsAsync(9, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            results.Status.Should().Be(SearchJobStatus.Running);
            results.Total.Should().Be(1);
            results.Results.Should().ContainSingle();
            results.Results[0].DescriptionLink.Should().Be("https://example.com/details");
            results.Results[0].FileName.Should().Be("FileName");
            results.Results[0].FileSize.Should().Be(12345);
            results.Results[0].FileUrl.Should().Be("magnet:?xt=urn:btih:hash");
            results.Results[0].Leechers.Should().Be(6);
            results.Results[0].Seeders.Should().Be(7);
            results.Results[0].SiteUrl.Should().Be("https://example.com");
            results.Results[0].EngineName.Should().Be("Engine");
            results.Results[0].PublishedOn.Should().Be(1700000000);
        }

        [Fact]
        public async Task GIVEN_LimitAndOffset_WHEN_GetSearchResults_THEN_ShouldGETWithAllParamsInOrder()
        {
            _handler.Responder = (req, _) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/search/results?id=9&limit=50&offset=100");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            };

            var results = (await _target.GetSearchResultsAsync(9, limit: 50, offset: 100, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            results.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetSearchResults_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("err")
            });

            var result = await _target.GetSearchResultsAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "err");
        }

        [Fact]
        public async Task GIVEN_NotFound_WHEN_GetSearchResults_THEN_ShouldReturnSearchMissingFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.GetSearchResultsAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.NotFound,
                statusCode: HttpStatusCode.NotFound,
                userMessage: "missing");

            failure.TryGetReason<SearchFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(SearchFailureReason.SearchMissing);
        }

        [Fact]
        public async Task GIVEN_NotFoundWithoutBody_WHEN_GetSearchResults_THEN_ShouldUseDefaultSearchMissingMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));

            var result = await _target.GetSearchResultsAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.NotFound,
                statusCode: HttpStatusCode.NotFound,
                userMessage: "The search job could not be found.");

            failure.TryGetReason<SearchFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(SearchFailureReason.SearchMissing);
        }

        [Fact]
        public async Task GIVEN_Conflict_WHEN_GetSearchResults_THEN_ShouldReturnOffsetOutOfRangeFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("offset")
            });

            var result = await _target.GetSearchResultsAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "offset");

            failure.TryGetReason<SearchFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(SearchFailureReason.OffsetOutOfRange);
        }

        [Fact]
        public async Task GIVEN_ConflictWithoutBody_WHEN_GetSearchResults_THEN_ShouldUseDefaultOffsetOutOfRangeMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict));

            var result = await _target.GetSearchResultsAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.Conflict,
                statusCode: HttpStatusCode.Conflict,
                userMessage: "The requested search result range is invalid.");

            failure.TryGetReason<SearchFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(SearchFailureReason.OffsetOutOfRange);
        }

        [Fact]
        public async Task GIVEN_Id_WHEN_DeleteSearch_THEN_ShouldPOSTFormWithId()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/delete");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("id=3");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DeleteSearchAsync(3, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_DeleteSearch_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.DeleteSearchAsync(3, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_NotFound_WHEN_DeleteSearch_THEN_ShouldReturnSearchMissingFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.DeleteSearchAsync(3, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.NotFound,
                statusCode: HttpStatusCode.NotFound,
                userMessage: "missing");

            failure.TryGetReason<SearchFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(SearchFailureReason.SearchMissing);
        }

        [Fact]
        public async Task GIVEN_NotFoundWithoutBody_WHEN_DeleteSearch_THEN_ShouldUseDefaultSearchMissingMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));

            var result = await _target.DeleteSearchAsync(3, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.NotFound,
                statusCode: HttpStatusCode.NotFound,
                userMessage: "The search job could not be found.");

            failure.TryGetReason<SearchFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(SearchFailureReason.SearchMissing);
        }

        [Fact]
        public async Task GIVEN_Request_WHEN_GetSearchPlugins_THEN_ShouldGETAndReturnUnexpectedResponseOnBadJson()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            });

            var list = (await _target.GetSearchPluginsAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();
            list.Should().NotBeNull();
            list.Count.Should().Be(0);

            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetSearchPluginsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_SearchPluginsPayload_WHEN_GetSearchPlugins_THEN_ShouldDeserializePlugins()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    [
                        {
                            "enabled": true,
                            "fullName": "FullName",
                            "name": "Name",
                            "supportedCategories":
                            [
                                {
                                    "id": "movies",
                                    "name": "Movies"
                                }
                            ],
                            "url": "https://example.com/plugin",
                            "version": "1.0"
                        }
                    ]
                    """)
            });

            var list = (await _target.GetSearchPluginsAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            list.Should().ContainSingle();
            list[0].Enabled.Should().BeTrue();
            list[0].FullName.Should().Be("FullName");
            list[0].Name.Should().Be("Name");
            list[0].SupportedCategories.Should().ContainSingle();
            list[0].SupportedCategories[0].Id.Should().Be("movies");
            list[0].SupportedCategories[0].Name.Should().Be("Movies");
            list[0].Url.Should().Be("https://example.com/plugin");
            list[0].Version.Should().Be("1.0");
        }

        [Fact]
        public async Task GIVEN_ForbiddenWithMessage_WHEN_GetSearchPlugins_THEN_ShouldReturnSearchUnavailableFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("plugins unavailable")
            });

            var result = await _target.GetSearchPluginsAsync(cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.AccessDenied,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "plugins unavailable");

            failure.TryGetReason<SearchFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(SearchFailureReason.SearchUnavailable);
        }

        [Fact]
        public async Task GIVEN_ForbiddenWithoutMessage_WHEN_GetSearchPlugins_THEN_ShouldUseGenericForbiddenFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden));

            var result = await _target.GetSearchPluginsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.AuthenticationRequired,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "Authentication is required.");
        }

        [Fact]
        public async Task GIVEN_Sources_WHEN_InstallSearchPlugins_THEN_ShouldPOSTPipeSeparatedSources()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/installPlugin");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("sources=s1%7Cs2");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.InstallSearchPluginsAsync(sources: ["s1", "s2"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_InstallSearchPlugins_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("conflict")
            });

            var result = await _target.InstallSearchPluginsAsync(sources: ["s"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "conflict");
        }

        [Fact]
        public async Task GIVEN_Names_WHEN_UninstallSearchPlugins_THEN_ShouldPOSTPipeSeparatedNames()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/uninstallPlugin");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("names=p1%7Cp2");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.UninstallSearchPluginsAsync(names: ["p1", "p2"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_UninstallSearchPlugins_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("nope")
            });

            var result = await _target.UninstallSearchPluginsAsync(names: ["p"], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "nope");
        }

        [Fact]
        public async Task GIVEN_Names_WHEN_EnableSearchPlugins_THEN_ShouldPOSTNamesAndEnableTrue()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/enablePlugin");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("names=p1%7Cp2&enable=true");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.EnableSearchPluginsAsync(names: ["p1", "p2"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Names_WHEN_DisableSearchPlugins_THEN_ShouldPOSTNamesAndEnableFalse()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/enablePlugin");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("names=p1%7Cp2&enable=false");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DisableSearchPluginsAsync(names: ["p1", "p2"], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_EnableOrDisableSearchPlugins_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("err")
            });

            var result1 = await _target.EnableSearchPluginsAsync(names: ["p"], cancellationToken: TestContext.Current.CancellationToken);
            result1.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "err");

            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("err2")
            });

            var result2 = await _target.DisableSearchPluginsAsync(names: ["p"], cancellationToken: TestContext.Current.CancellationToken);
            result2.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "err2");
        }

        [Fact]
        public async Task GIVEN_PluginAndUrl_WHEN_DownloadSearchResult_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/downloadTorrent");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("pluginName=qb&torrentUrl=http%3A%2F%2Fexample.com");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DownloadSearchResultAsync("qb", "http://example.com", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Request_WHEN_UpdateSearchPlugins_THEN_ShouldPOSTAndNotThrow()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/search/updatePlugins");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            };

            await _target.UpdateSearchPluginsAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_UpdateSearchPlugins_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.UpdateSearchPluginsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadGateway, userMessage: "bad");
        }
    }
}
