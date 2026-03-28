using AwesomeAssertions;
using QBittorrent.ApiClient.Models;
using System.Net;

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
                                "IsLoading": true,
                                "lastBuildDate": "2024-01-01",
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
            var item = dict["FeedPath"];
            item.HasError.Should().BeFalse();
            item.IsLoading.Should().BeTrue();
            item.LastBuildDate.Should().Be("2024-01-01");
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

                var json = decoded.Substring("ruleName=r1&ruleDef=".Length);
                var expectedJson = System.Text.Json.JsonSerializer.Serialize(new AutoDownloadingRule());

                json.Should().Be(expectedJson);

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetRssAutoDownloadingRuleAsync("r1", new AutoDownloadingRule(), cancellationToken: TestContext.Current.CancellationToken);
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
    }
}
