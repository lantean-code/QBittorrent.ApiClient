using System.Text.Json;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test.Models
{
    public sealed class TorrentMetadataTests
    {
        [Fact]
        public void GIVEN_DefaultConstruction_WHEN_ReadingOptionalCollections_THEN_ShouldReturnEmptyCollections()
        {
            var result = new TorrentMetadata(
                infoHashV1: null,
                infoHashV2: null,
                hash: null,
                info: new TorrentMetadataInfo(
                    name: "Name",
                    length: 99,
                    pieceLength: 16,
                    piecesNum: 7,
                    @private: true,
                    files: null),
                trackers: null,
                webSeeds: null,
                createdBy: null,
                creationDate: null,
                comment: null);

            result.Trackers.Should().BeEmpty();
            result.WebSeeds.Should().BeEmpty();
            result.Info.Files.Should().BeEmpty();
        }

        [Fact]
        public void GIVEN_NullName_WHEN_ConstructingInfo_THEN_ShouldThrowJsonException()
        {
            var action = () => new TorrentMetadataInfo(
                name: null,
                length: 99,
                pieceLength: 16,
                piecesNum: 7,
                @private: true,
                files: []);

            action.Should().Throw<JsonException>()
                .WithMessage("The torrent metadata info payload did not include a valid name.");
        }
    }
}
