using System.Text.Json;
using AwesomeAssertions;
using QBittorrent.ApiClient.Converters;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test.Converters
{
    public class SaveLocationJsonConverterTests
    {
        private static JsonSerializerOptions CreateOptions()
        {
            var o = new JsonSerializerOptions();
            o.Converters.Add(new SaveLocationJsonConverter());
            return o;
        }

        // -------- Read --------

        [Fact]
        public void GIVEN_String_WHEN_Read_THEN_ShouldReturnCustomPath()
        {
            var options = CreateOptions();
            var json = "\"/downloads\"";

            var result = JsonSerializer.Deserialize<SaveLocation>(json, options);

            result.Should().NotBeNull();
            result.Kind.Should().Be(SaveLocationKind.CustomPath);
            result.SavePath.Should().Be("/downloads");
        }

        [Fact]
        public void GIVEN_NumberZero_WHEN_Read_THEN_ShouldReturnWatchedFolder()
        {
            var options = CreateOptions();
            var json = "0";

            var result = JsonSerializer.Deserialize<SaveLocation>(json, options);

            result.Should().NotBeNull();
            result.Kind.Should().Be(SaveLocationKind.WatchedFolder);
            result.SavePath.Should().BeNull();
        }

        [Fact]
        public void GIVEN_NumberOne_WHEN_Read_THEN_ShouldReturnDefaultFolder()
        {
            var options = CreateOptions();
            var json = "1";

            var result = JsonSerializer.Deserialize<SaveLocation>(json, options);

            result.Should().NotBeNull();
            result.Kind.Should().Be(SaveLocationKind.DefaultFolder);
            result.SavePath.Should().BeNull();
        }

        [Fact]
        public void GIVEN_UnsupportedToken_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();
            var json = "true"; // bool token is not supported

            var act = () =>
            {
                JsonSerializer.Deserialize<SaveLocation>(json, options);
            };

            var ex = act.Should().Throw<JsonException>();
            ex.Which.Message.Should().Contain("Unsupported token type");
        }

        // -------- Write --------

        [Fact]
        public void GIVEN_WatchedFolder_WHEN_Write_THEN_ShouldEmitZero()
        {
            var options = CreateOptions();
            var value = SaveLocation.Create(0);

            var json = JsonSerializer.Serialize(value, options);

            json.Should().Be("0");
        }

        [Fact]
        public void GIVEN_DefaultFolder_WHEN_Write_THEN_ShouldEmitOne()
        {
            var options = CreateOptions();
            var value = SaveLocation.Create(1);

            var json = JsonSerializer.Serialize(value, options);

            json.Should().Be("1");
        }

        [Fact]
        public void GIVEN_CustomPath_WHEN_Write_THEN_ShouldEmitJsonString()
        {
            var options = CreateOptions();
            var value = SaveLocation.Create("/data/films");

            var json = JsonSerializer.Serialize(value, options);

            json.Should().Be("\"/data/films\"");
        }

        // -------- Round-trip sanity --------

        [Fact]
        public void GIVEN_PathString_WHEN_RoundTrip_THEN_ShouldPreserveCustomPath()
        {
            var options = CreateOptions();
            var original = SaveLocation.Create("/data");

            var json = JsonSerializer.Serialize(original, options);
            var round = JsonSerializer.Deserialize<SaveLocation>(json, options);

            round?.Kind.Should().Be(SaveLocationKind.CustomPath);
            round?.SavePath.Should().Be("/data");
        }

        [Fact]
        public void GIVEN_Zero_WHEN_RoundTrip_THEN_ShouldStayWatchedFolder()
        {
            var options = CreateOptions();
            var original = SaveLocation.Create(0);

            var json = JsonSerializer.Serialize(original, options);
            var round = JsonSerializer.Deserialize<SaveLocation>(json, options);

            round?.Kind.Should().Be(SaveLocationKind.WatchedFolder);
            round?.SavePath.Should().BeNull();
        }

        [Fact]
        public void GIVEN_One_WHEN_RoundTrip_THEN_ShouldStayDefaultFolder()
        {
            var options = CreateOptions();
            var original = SaveLocation.Create(1);

            var json = JsonSerializer.Serialize(original, options);
            var round = JsonSerializer.Deserialize<SaveLocation>(json, options);

            round?.Kind.Should().Be(SaveLocationKind.DefaultFolder);
            round?.SavePath.Should().BeNull();
        }
    }
}
