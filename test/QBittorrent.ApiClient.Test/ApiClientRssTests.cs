using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientRssTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientRssTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_Path_WHEN_AddRssFolder_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/addFolder");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("path=%2Ffeeds%2Ftv");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.AddRssFolderAsync("/feeds/tv", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_AddRssFolder_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("exists")
            });

            var result = await _target.AddRssFolderAsync("/x", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "exists");
        }

        [Fact]
        public async Task GIVEN_UrlOnly_WHEN_AddRssFeed_THEN_ShouldPOSTUrlWithEmptyPath()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/addFeed");
                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                decoded.Should().Be("url=http://feed&path=");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.AddRssFeedAsync("http://feed", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_UrlAndPath_WHEN_AddRssFeed_THEN_ShouldIncludeBoth()
        {
            _handler.Responder = async (req, ct) =>
            {
                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                decoded.Should().Be("url=http://feed&path=/podcasts");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.AddRssFeedAsync("http://feed", "/podcasts", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersionAndRefreshInterval_WHEN_AddRssFeed_THEN_ShouldIncludeRefreshInterval()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.5");

                    case "/rss/addFeed":
                        var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                        body.Should().Be("url=http://feed&path=/podcasts&refreshInterval=60");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            await _target.AddRssFeedAsync("http://feed", "/podcasts", 60, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeRssFeedRefreshIntervalAndRefreshInterval_WHEN_AddRssFeed_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var addFeedRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/rss/addFeed":
                        addFeedRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.AddRssFeedAsync("http://feed", "/podcasts", 60, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.4 does not support RSS feed refresh intervals.");
            addFeedRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailureAndRefreshInterval_WHEN_AddRssFeed_THEN_ShouldReturnProbeFailure()
        {
            var addFeedRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/rss/addFeed":
                        addFeedRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.AddRssFeedAsync("http://feed", "/podcasts", 60, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            addFeedRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_AddRssFeed_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.AddRssFeedAsync("u", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_Path_WHEN_RemoveRssItem_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/removeItem");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("path=%2Ffeeds%2Ftv");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RemoveRssItemAsync("/feeds/tv", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ItemAndDest_WHEN_MoveRssItem_THEN_ShouldPOSTBoth()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/moveItem");
                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                decoded.Should().Be("itemPath=/feeds/tv&destPath=/feeds/news");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.MoveRssItemAsync("/feeds/tv", "/feeds/news", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_MoveRssItem_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("nope")
            });

            var result = await _target.MoveRssItemAsync("/a", "/b", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "nope");
        }

        [Fact]
        public async Task GIVEN_PathAndUrl_WHEN_SetRssFeedUrl_THEN_ShouldPOSTBoth()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/setFeedURL");
                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                decoded.Should().Be("path=/feeds/tv&url=http://example.com");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetRssFeedUrlAsync("/feeds/tv", "http://example.com", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_SetRssFeedRefreshInterval_THEN_ShouldPostInterval()
        {
            _handler.Responder = async (req, ct) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return CreateResponse(HttpStatusCode.OK, "2.11.5");

                    case "/rss/setFeedRefreshInterval":
                        var body = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                        body.Should().Be("path=/feeds/tv&refreshInterval=120");
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            (await _target.SetRssFeedRefreshIntervalAsync("/feeds/tv", 120, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeRssFeedRefreshInterval_WHEN_SetRssFeedRefreshInterval_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var setIntervalRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.4"));

                    case "/rss/setFeedRefreshInterval":
                        setIntervalRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.SetRssFeedRefreshIntervalAsync("/feeds/tv", 120, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.4 does not support RSS feed refresh intervals.");
            setIntervalRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_SetRssFeedRefreshInterval_THEN_ShouldReturnProbeFailure()
        {
            var setIntervalRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/rss/setFeedRefreshInterval":
                        setIntervalRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.SetRssFeedRefreshIntervalAsync("/feeds/tv", 120, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            setIntervalRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetRssFeedUrl_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.SetRssFeedUrlAsync("/feeds/tv", "http://example.com", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_NoFlag_WHEN_GetAllRssItems_THEN_ShouldGETWithoutQueryAndReturnDict()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/rss/items");
                req.RequestUri!.Query.Should().BeEmpty();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            };

            var dict = (await _target.GetAllRssItemsAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            dict.Should().NotBeNull();
            dict.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_WithDataTrue_WHEN_GetAllRssItems_THEN_ShouldQueryWithTrueCapitalized()
        {
            _handler.Responder = (req, _) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/items?withData=True");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            };

            var dict = (await _target.GetAllRssItemsAsync(true, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            dict.Should().NotBeNull();
            dict.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_RssItemsPayload_WHEN_GetAllRssItems_THEN_ShouldDeserializeItemsAndArticles()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/items?withData=True");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "FeedPath":
                            {
                                "articles":
                                [
                                    {
                                        "category": "Category",
                                        "comments": "Comments",
                                        "date": "2024-01-01",
                                        "description": "Description",
                                        "id": "ArticleId",
                                        "link": "https://example.com/article",
                                        "thumbnail": "https://example.com/image.png",
                                        "title": "Title",
                                        "torrentURL": "magnet:?xt=urn:btih:hash",
                                        "isRead": true
                                    }
                                ],
                                "hasError": false,
                                "isLoading": true,
                                "lastBuildDate": "2024-01-01",
                                "refreshInterval": 60,
                                "title": "FeedTitle",
                                "uid": "Uid",
                                "url": "https://example.com/feed"
                            }
                        }
                        """)
                });
            };

            var dict = (await _target.GetAllRssItemsAsync(true, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            dict.Should().ContainKey("FeedPath");
            var item = dict["FeedPath"].Should().BeOfType<RssFeedItem>().Subject;
            item.HasError.Should().BeFalse();
            item.IsLoading.Should().BeTrue();
            item.LastBuildDate.Should().Be("2024-01-01");
            item.RefreshInterval.Should().Be(60);
            item.Title.Should().Be("FeedTitle");
            item.Uid.Should().Be("Uid");
            item.Url.Should().Be("https://example.com/feed");
            item.Articles.Should().ContainSingle();
            item.Articles![0].Category.Should().Be("Category");
            item.Articles[0].Comments.Should().Be("Comments");
            item.Articles[0].Date.Should().Be("2024-01-01");
            item.Articles[0].Description.Should().Be("Description");
            item.Articles[0].Id.Should().Be("ArticleId");
            item.Articles[0].Link.Should().Be("https://example.com/article");
            item.Articles[0].Thumbnail.Should().Be("https://example.com/image.png");
            item.Articles[0].Title.Should().Be("Title");
            item.Articles[0].TorrentURL.Should().Be("magnet:?xt=urn:btih:hash");
            item.Articles[0].IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task GIVEN_RssFolderTree_WHEN_GetAllRssItems_THEN_ShouldDeserializeNestedFoldersAndFeeds()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/items?withData=True");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "Folder":
                            {
                                "FeedPath":
                                {
                                    "articles": [],
                                    "hasError": false,
                                    "isLoading": false,
                                    "lastBuildDate": "2024-01-02",
                                    "refreshInterval": 120,
                                    "title": "NestedFeed",
                                    "uid": "NestedUid",
                                    "url": "https://example.com/nested"
                                }
                            }
                        }
                        """)
                });
            };

            var dict = (await _target.GetAllRssItemsAsync(true, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            dict.Should().ContainKey("Folder");
            var folder = dict["Folder"].Should().BeOfType<RssFolderItem>().Subject;
            folder.Children.Should().ContainKey("FeedPath");

            var feed = folder.Children["FeedPath"].Should().BeOfType<RssFeedItem>().Subject;
            feed.Uid.Should().Be("NestedUid");
            feed.Url.Should().Be("https://example.com/nested");
            feed.Title.Should().Be("NestedFeed");
            feed.LastBuildDate.Should().Be("2024-01-02");
            feed.RefreshInterval.Should().Be(120);
            feed.IsLoading.Should().BeFalse();
            feed.HasError.Should().BeFalse();
            feed.Articles.Should().NotBeNull();
            feed.Articles.Should().BeEmpty();
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetAllRssItems_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("err")
            });

            var result = await _target.GetAllRssItemsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "err");
        }

        [Fact]
        public async Task GIVEN_ItemPathOnly_WHEN_MarkRssItemAsRead_THEN_ShouldPOSTOnlyItemPath()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/markAsRead");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("itemPath=%2Ffeeds%2Ftv");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.MarkRssItemAsReadAsync("/feeds/tv", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ArticleId_WHEN_MarkRssItemAsRead_THEN_ShouldIncludeArticleId()
        {
            _handler.Responder = async (req, ct) =>
            {
                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                decoded.Should().Be("itemPath=/feeds/tv&articleId=a1");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.MarkRssItemAsReadAsync("/feeds/tv", "a1", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_ItemPath_WHEN_RefreshRssItem_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/refreshItem");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("itemPath=%2Ffeeds%2Ftv");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RefreshRssItemAsync("/feeds/tv", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_Rule_WHEN_SetRssAutoDownloadingRule_THEN_ShouldPOSTRuleNameAndRuleDefJson()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/setRule");

                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                decoded.Should().StartWith("ruleName=r1&ruleDef=");

                var json = decoded["ruleName=r1&ruleDef=".Length..];
                var expectedJson = System.Text.Json.JsonSerializer.Serialize(new AutoDownloadingRule(), SerializerOptions.Options);

                json.Should().Be(expectedJson);

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetRssAutoDownloadingRuleAsync("r1", new AutoDownloadingRule(), cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_RuleWithRatioLimit_WHEN_SetRssAutoDownloadingRule_THEN_ShouldSerializeRatioLimitAsJsonNumber()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/setRule");

                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                var json = decoded["ruleName=r1&ruleDef=".Length..];

                json.Should().Contain("\"ratio_limit\":1.23456789012345");
                json.Should().NotContain("\"ratio_limit\":\"1.23456789012345\"");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var rule = new AutoDownloadingRule
            {
                TorrentParams = new AutoDownloadingRuleTorrentParams
                {
                    RatioLimit = 1.23456789012345
                }
            };

            (await _target.SetRssAutoDownloadingRuleAsync("r1", rule, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_RuleWithEnumBackedTorrentParams_WHEN_SetRssAutoDownloadingRule_THEN_ShouldSerializeEnumValuesAsStrings()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/setRule");

                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                var json = decoded["ruleName=r1&ruleDef=".Length..];

                json.Should().Contain("\"torrentContentLayout\":\"Subfolder\"");
                json.Should().Contain("\"operating_mode\":\"Forced\"");
                json.Should().Contain("\"content_layout\":\"NoSubfolder\"");
                json.Should().Contain("\"stop_condition\":\"FilesChecked\"");
                json.Should().Contain("\"share_limit_action\":\"RemoveWithContent\"");
                json.Should().NotContain("\"operating_mode\":1");
                json.Should().NotContain("\"content_layout\":2");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var rule = new AutoDownloadingRule
            {
                TorrentContentLayout = TorrentContentLayout.Subfolder,
                TorrentParams = new AutoDownloadingRuleTorrentParams
                {
                    OperatingMode = TorrentOperatingMode.Forced,
                    ContentLayout = TorrentContentLayout.NoSubfolder,
                    StopCondition = StopCondition.FilesChecked,
                    ShareLimitAction = ShareLimitAction.RemoveWithContent
                }
            };

            (await _target.SetRssAutoDownloadingRuleAsync("r1", rule, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_RuleWithCompleteTorrentParams_WHEN_SetRssAutoDownloadingRule_THEN_ShouldSerializeQbittorrentTorrentParams()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/setRule");

                var decoded = Uri.UnescapeDataString(await req.Content!.ReadAsStringAsync(ct));
                var json = decoded["ruleName=r1&ruleDef=".Length..];

                using var jsonDocument = System.Text.Json.JsonDocument.Parse(json);
                var torrentParams = jsonDocument.RootElement.GetProperty("torrentParams");
                torrentParams.GetProperty("use_download_path").GetBoolean().Should().BeTrue();
                torrentParams.GetProperty("add_to_top_of_queue").GetBoolean().Should().BeFalse();
                torrentParams.GetProperty("ssl_certificate").GetString().Should().Be("cert");
                torrentParams.GetProperty("ssl_private_key").GetString().Should().Be("key");
                torrentParams.GetProperty("ssl_dh_params").GetString().Should().Be("dh");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var rule = new AutoDownloadingRule
            {
                TorrentParams = new AutoDownloadingRuleTorrentParams
                {
                    UseDownloadPath = true,
                    AddToTopOfQueue = false,
                    SslCertificate = "cert",
                    SslPrivateKey = "key",
                    SslDhParams = "dh"
                }
            };

            (await _target.SetRssAutoDownloadingRuleAsync("r1", rule, cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
        }

        [Fact]
        public async Task GIVEN_RuleNames_WHEN_RenameRssAutoDownloadingRule_THEN_ShouldPOSTBothNames()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/renameRule");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("ruleName=old&newRuleName=new");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RenameRssAutoDownloadingRuleAsync("old", "new", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_RuleName_WHEN_RemoveRssAutoDownloadingRule_THEN_ShouldPOSTName()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/removeRule");
                (await req.Content!.ReadAsStringAsync(ct)).Should().Be("ruleName=dead");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.RemoveRssAutoDownloadingRuleAsync("dead", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_OKOrBadJson_WHEN_GetAllRssAutoDownloadingRules_THEN_ShouldDeserializeOrReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            var dict = (await _target.GetAllRssAutoDownloadingRulesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();
            dict.Should().NotBeNull();
            dict.Count.Should().Be(0);

            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetAllRssAutoDownloadingRulesAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_RuleWithQbittorrentTorrentParams_WHEN_GetAllRssAutoDownloadingRules_THEN_ShouldDeserializeTorrentParams()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                        "rule1":
                        {
                            "torrentParams":
                            {
                                "category": "category",
                                "tags": ["tag"],
                                "save_path": "/save",
                                "use_download_path": true,
                                "download_path": "/download",
                                "operating_mode": "Forced",
                                "add_to_top_of_queue": false,
                                "stopped": true,
                                "stop_condition": "FilesChecked",
                                "skip_checking": true,
                                "content_layout": "NoSubfolder",
                                "use_auto_tmm": true,
                                "upload_limit": 10,
                                "download_limit": 20,
                                "seeding_time_limit": 30,
                                "inactive_seeding_time_limit": 40,
                                "share_limit_action": "RemoveWithContent",
                                "ratio_limit": 1.5,
                                "ssl_certificate": "cert",
                                "ssl_private_key": "key",
                                "ssl_dh_params": "dh"
                            }
                        }
                    }
                    """)
            });

            var dict = (await _target.GetAllRssAutoDownloadingRulesAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            var torrentParams = dict["rule1"].TorrentParams;
            torrentParams.Category.Should().Be("category");
            torrentParams.Tags.Should().Equal("tag");
            torrentParams.SavePath.Should().Be("/save");
            torrentParams.UseDownloadPath.Should().BeTrue();
            torrentParams.DownloadPath.Should().Be("/download");
            torrentParams.OperatingMode.Should().Be(TorrentOperatingMode.Forced);
            torrentParams.AddToTopOfQueue.Should().BeFalse();
            torrentParams.Stopped.Should().BeTrue();
            torrentParams.StopCondition.Should().Be(StopCondition.FilesChecked);
            torrentParams.SkipChecking.Should().BeTrue();
            torrentParams.ContentLayout.Should().Be(TorrentContentLayout.NoSubfolder);
            torrentParams.UseAutoTmm.Should().BeTrue();
            torrentParams.UploadLimit.Should().Be(10);
            torrentParams.DownloadLimit.Should().Be(20);
            torrentParams.SeedingTimeLimit.Should().Be(30);
            torrentParams.InactiveSeedingTimeLimit.Should().Be(40);
            torrentParams.ShareLimitAction.Should().Be(ShareLimitAction.RemoveWithContent);
            torrentParams.RatioLimit.Should().Be(1.5);
            torrentParams.SslCertificate.Should().Be("cert");
            torrentParams.SslPrivateKey.Should().Be("key");
            torrentParams.SslDhParams.Should().Be("dh");
        }

        [Fact]
        public async Task GIVEN_RuleName_WHEN_GetRssMatchingArticles_THEN_ShouldGETAndReturnDictionaryOfLists()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/rss/matchingArticles?ruleName=myrule");

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"feed1\":[\"a\",\"b\"]}")
                });
            };

            var dict = (await _target.GetRssMatchingArticlesAsync("myrule", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            dict.Should().NotBeNull();
            dict.Count.Should().Be(1);
            dict["feed1"].Count.Should().Be(2);
            dict["feed1"][0].Should().Be("a");
            dict["feed1"][1].Should().Be("b");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetRssMatchingArticles_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("fail")
            });

            var result = await _target.GetRssMatchingArticlesAsync("x", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadGateway, userMessage: "fail");
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
