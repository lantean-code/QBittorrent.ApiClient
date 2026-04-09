using QBittorrent.ApiClient.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBittorrent.ApiClient.Converters
{
    internal sealed class RssItemJsonConverter : JsonConverter<RssItem>
    {
        public override RssItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            return ReadItem(jsonDocument.RootElement, options);
        }

        public override void Write(Utf8JsonWriter writer, RssItem value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is RssFolderItem folder)
            {
                foreach (var child in folder.Children)
                {
                    writer.WritePropertyName(child.Key);
                    JsonSerializer.Serialize(writer, child.Value, options);
                }

                writer.WriteEndObject();
                return;
            }

            if (value is not RssFeedItem feed)
            {
                throw new JsonException($"Unsupported RSS item type: {value.GetType().Name}");
            }

            if (feed.Uid is not null)
            {
                writer.WriteString("uid", feed.Uid);
            }

            if (feed.Url is not null)
            {
                writer.WriteString("url", feed.Url);
            }

            if (feed.RefreshInterval is not null)
            {
                writer.WriteNumber("refreshInterval", feed.RefreshInterval.Value);
            }

            if (feed.Title is not null)
            {
                writer.WriteString("title", feed.Title);
            }

            if (feed.LastBuildDate is not null)
            {
                writer.WriteString("lastBuildDate", feed.LastBuildDate);
            }

            if (feed.IsLoading is not null)
            {
                writer.WriteBoolean("isLoading", feed.IsLoading.Value);
            }

            if (feed.HasError is not null)
            {
                writer.WriteBoolean("hasError", feed.HasError.Value);
            }

            if (feed.Articles is not null)
            {
                writer.WritePropertyName("articles");
                JsonSerializer.Serialize(writer, feed.Articles, options);
            }

            writer.WriteEndObject();
        }

        private static RssItem ReadItem(JsonElement item, JsonSerializerOptions options)
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException("RSS items must be JSON objects.");
            }

            if (IsFeed(item))
            {
                return item.Deserialize<RssFeedItem>(options)!;
            }

            var children = new Dictionary<string, RssItem>();
            foreach (var child in item.EnumerateObject())
            {
                children[child.Name] = ReadItem(child.Value, options);
            }

            return new RssFolderItem(children.AsReadOnly());
        }

        private static bool IsFeed(JsonElement item)
        {
            return item.TryGetProperty("uid", out var uid)
                && (uid.ValueKind == JsonValueKind.String)
                && item.TryGetProperty("url", out var url)
                && (url.ValueKind == JsonValueKind.String);
        }
    }
}
