using AwesomeAssertions;
using QBittorrent.ApiClient.Converters;
using QBittorrent.ApiClient.Models;
using System.Text.Json;

namespace QBittorrent.ApiClient.Test.Converters
{
    public class DirectoryContentEntryTypeJsonConverterTests
    {
        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DirectoryContentEntryTypeJsonConverter());
            return options;
        }

        [Fact]
        public void GIVEN_DirectoryToken_WHEN_Read_THEN_ShouldReturnDirectory()
        {
            var options = CreateOptions();

            var result = JsonSerializer.Deserialize<DirectoryContentEntryType>("\"dir\"", options);

            result.Should().Be(DirectoryContentEntryType.Directory);
        }

        [Fact]
        public void GIVEN_FileToken_WHEN_Read_THEN_ShouldReturnFile()
        {
            var options = CreateOptions();

            var result = JsonSerializer.Deserialize<DirectoryContentEntryType>("\"file\"", options);

            result.Should().Be(DirectoryContentEntryType.File);
        }

        [Fact]
        public void GIVEN_UnknownToken_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<DirectoryContentEntryType>("\"other\"", options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_Directory_WHEN_Write_THEN_ShouldEmitDirectoryToken()
        {
            var options = CreateOptions();

            var json = JsonSerializer.Serialize(DirectoryContentEntryType.Directory, options);

            json.Should().Be("\"dir\"");
        }

        [Fact]
        public void GIVEN_File_WHEN_Write_THEN_ShouldEmitFileToken()
        {
            var options = CreateOptions();

            var json = JsonSerializer.Serialize(DirectoryContentEntryType.File, options);

            json.Should().Be("\"file\"");
        }

        [Fact]
        public void GIVEN_UnsupportedEnumValue_WHEN_Write_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Serialize((DirectoryContentEntryType)999, options);

            action.Should().Throw<JsonException>();
        }
    }
}
