using System.Text.Json;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class SerializerOptionsTests
    {
        [Fact]
        public void GIVEN_BuildInfoPayload_WHEN_DeserializingWithGeneratedTypeInfo_THEN_ShouldDeserialize()
        {
            var result = JsonSerializer.Deserialize(
                """
                {
                    "bitness": 64,
                    "platform": "linux"
                }
                """,
                SerializerOptions.GetTypeInfo<BuildInfo>());

            result.Should().NotBeNull();
            result!.Bitness.Should().Be(64);
            result.Platform.Should().Be(BuildPlatform.Linux);
            SerializerOptions.Context.Should().NotBeNull();
        }

        [Fact]
        public void GIVEN_ClientDataPatch_WHEN_SerializingAndDeserializingWithSharedOptions_THEN_ShouldPreserveDeleteSemantics()
        {
            var payload = new Dictionary<string, JsonElement?>
            {
                ["QbtMud.AppSettings.State.v1"] = JsonSerializer.SerializeToElement(new { notifications = true }),
                ["QbtMud.Search.Jobs"] = null
            };

            var json = JsonSerializer.Serialize(payload, SerializerOptions.Options);
            var roundTrip = JsonSerializer.Deserialize<Dictionary<string, JsonElement?>>(json, SerializerOptions.Options);

            roundTrip.Should().NotBeNull();
            roundTrip!.Should().ContainKey("QbtMud.AppSettings.State.v1");
            roundTrip["QbtMud.AppSettings.State.v1"]!.Value.GetProperty("notifications").GetBoolean().Should().BeTrue();
            roundTrip.Should().ContainKey("QbtMud.Search.Jobs");
            roundTrip["QbtMud.Search.Jobs"].Should().BeNull();
        }

        [Fact]
        public void GIVEN_SearchStartPayload_WHEN_DeserializingWithGeneratedTypeInfo_THEN_ShouldDeserialize()
        {
            var result = JsonSerializer.Deserialize(
                """
                {
                    "id": 42
                }
                """,
                SerializerOptions.GetTypeInfo<SearchStartResult>());

            result.Should().NotBeNull();
            result!.Id.Should().Be(42);
        }
    }
}
