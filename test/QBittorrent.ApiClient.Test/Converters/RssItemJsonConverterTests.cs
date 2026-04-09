using AwesomeAssertions;
using QBittorrent.ApiClient.Models;
using System.Text.Json;

namespace QBittorrent.ApiClient.Test.Converters
{
    public class RssItemJsonConverterTests
    {
        private sealed record UnsupportedRssItem : RssItem
        {
        }

        [Fact]
        public void GIVEN_MinimalFeedJson_WHEN_Deserialize_THEN_ShouldReturnFeedNode()
        {
            const string json = """
                {
                    "uid": "Uid",
                    "url": "https://example.com/feed"
                }
                """;

            var result = JsonSerializer.Deserialize<RssItem>(json);

            result.Should().NotBeNull();
            var feed = result.Should().BeOfType<RssFeedItem>().Subject;
            feed.Uid.Should().Be("Uid");
            feed.Url.Should().Be("https://example.com/feed");
            feed.RefreshInterval.Should().BeNull();
            feed.Title.Should().BeNull();
            feed.LastBuildDate.Should().BeNull();
            feed.IsLoading.Should().BeNull();
            feed.HasError.Should().BeNull();
            feed.Articles.Should().BeNull();
        }

        [Fact]
        public void GIVEN_FolderJson_WHEN_Deserialize_THEN_ShouldReturnFolderNode()
        {
            const string json = """
                {
                    "Feed":
                    {
                        "uid": "Uid",
                        "url": "https://example.com/feed",
                        "refreshInterval": 90
                    }
                }
                """;

            var result = JsonSerializer.Deserialize<RssItem>(json);

            result.Should().NotBeNull();
            var folder = result.Should().BeOfType<RssFolderItem>().Subject;
            folder.Children.Should().ContainKey("Feed");
            var feed = folder.Children["Feed"].Should().BeOfType<RssFeedItem>().Subject;
            feed.Uid.Should().Be("Uid");
            feed.RefreshInterval.Should().Be(90);
        }

        [Fact]
        public void GIVEN_NonObjectJson_WHEN_Deserialize_THEN_ShouldThrowJsonException()
        {
            const string json = "[]";

            var act = () => JsonSerializer.Deserialize<RssItem>(json)!;

            var ex = act.Should().Throw<JsonException>();
            ex.Which.Message.Should().Contain("RSS items must be JSON objects.");
        }

        [Fact]
        public void GIVEN_FeedNode_WHEN_Serialize_THEN_ShouldWriteUpstreamFieldNames()
        {
            var value = new RssFeedItem(
                [
                    new RssArticle("Category", "Comments", "2024-01-01", "Description", "ArticleId", "https://example.com/article", "https://example.com/image.png", "Title", "magnet:?xt=urn:btih:hash", true)
                ],
                false,
                true,
                "2024-01-02",
                120,
                "FeedTitle",
                "Uid",
                "https://example.com/feed");

            var json = JsonSerializer.Serialize<RssItem>(value);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            root.GetProperty("uid").GetString().Should().Be("Uid");
            root.GetProperty("url").GetString().Should().Be("https://example.com/feed");
            root.GetProperty("refreshInterval").GetInt64().Should().Be(120);
            root.GetProperty("title").GetString().Should().Be("FeedTitle");
            root.GetProperty("lastBuildDate").GetString().Should().Be("2024-01-02");
            root.GetProperty("isLoading").GetBoolean().Should().BeTrue();
            root.GetProperty("hasError").GetBoolean().Should().BeFalse();
            root.GetProperty("articles").GetArrayLength().Should().Be(1);
        }

        [Fact]
        public void GIVEN_EmptyLeafNode_WHEN_Serialize_THEN_ShouldWriteEmptyObject()
        {
            var value = new RssFolderItem(new Dictionary<string, RssItem>().AsReadOnly());

            var json = JsonSerializer.Serialize<RssItem>(value);

            json.Should().Be("{}");
        }

        [Fact]
        public void GIVEN_FolderNode_WHEN_Serialize_THEN_ShouldWriteNestedTree()
        {
            var value = new RssFolderItem(
                new Dictionary<string, RssItem>
                {
                    ["Feed"] = new RssFeedItem(null, null, null, null, null, null, "Uid", "https://example.com/feed")
                }.AsReadOnly());

            var json = JsonSerializer.Serialize<RssItem>(value);

            using var document = JsonDocument.Parse(json);
            var feed = document.RootElement.GetProperty("Feed");
            feed.GetProperty("uid").GetString().Should().Be("Uid");
            feed.GetProperty("url").GetString().Should().Be("https://example.com/feed");
        }

        [Fact]
        public void GIVEN_UnsupportedSubtype_WHEN_Serialize_THEN_ShouldThrowJsonException()
        {
            var value = (RssItem)new UnsupportedRssItem();

            var act = () => JsonSerializer.Serialize<RssItem>(value);

            var ex = act.Should().Throw<JsonException>();
            ex.Which.Message.Should().Contain("Unsupported RSS item type");
        }
    }
}
