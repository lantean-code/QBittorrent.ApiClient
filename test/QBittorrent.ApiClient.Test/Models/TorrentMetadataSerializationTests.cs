using System.Text.Json;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test.Models
{
    public class TorrentMetadataSerializationTests
    {
        private static JsonSerializerOptions CreateOptions()
        {
            return SerializerOptions.Options;
        }

        [Fact]
        public void GIVEN_MetadataPayload_WHEN_Read_THEN_ShouldDeserialize()
        {
            var options = CreateOptions();

            var result = JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "infohash_v1": "InfoHashV1",
                    "infohash_v2": "InfoHashV2",
                    "hash": "Hash",
                    "created_by": "CreatedBy",
                    "creation_date": 946684800,
                    "comment": "Comment",
                    "trackers":
                    [
                        {
                            "url": "udp://tracker",
                            "tier": 0
                        }
                    ],
                    "webseeds": [ "https://seed" ],
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            result.Should().NotBeNull();
            result!.InfoHashV1.Should().Be("InfoHashV1");
            result.InfoHashV2.Should().Be("InfoHashV2");
            result.Hash.Should().Be("Hash");
            result.Info.Name.Should().Be("Name");
            result.Info.Length.Should().Be(99);
            result.Info.PieceLength.Should().Be(16);
            result.Info.PiecesNum.Should().Be(7);
            result.Info.Private.Should().BeTrue();
            result.Info.Files.Should().ContainSingle();
            result.Trackers.Should().ContainSingle();
            result.WebSeeds.Should().ContainSingle();
            result.CreatedBy.Should().Be("CreatedBy");
            result.CreationDate.Should().Be(946684800);
            result.Comment.Should().Be("Comment");
            result.Info.Files.Should().ContainSingle();
        }

        [Fact]
        public void GIVEN_ArrayPayload_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>("[]", options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_InfoArray_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "info": []
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_FilePathNonString_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": 1,
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_FileLengthNonNumeric_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": "invalid"
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_TrackerCollectionNonArray_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "trackers": {},
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_TrackerTierNonNumeric_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "trackers":
                    [
                        {
                            "url": "udp://tracker",
                            "tier": "invalid"
                        }
                    ],
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_WebSeedsCollectionNonArray_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "webseeds": {},
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_WebSeedNonString_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "webseeds": [ 1 ],
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_NameNonString_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "info":
                    {
                        "name": 1,
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_InfoHashV1NonString_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "infohash_v1": 1,
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_CreationDateNonInteger_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "creation_date": 1.5,
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_CreationDateNonNumeric_WHEN_Read_THEN_ShouldThrowJsonException()
        {
            var options = CreateOptions();

            var action = () => JsonSerializer.Deserialize<TorrentMetadata>(
                """
                {
                    "creation_date": "invalid",
                    "info":
                    {
                        "name": "Name",
                        "length": 99,
                        "piece_length": 16,
                        "pieces_num": 7,
                        "private": true,
                        "files":
                        [
                            {
                                "path": "file.bin",
                                "length": 99
                            }
                        ]
                    }
                }
                """,
                options);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void GIVEN_MetadataWithAllOptionalFields_WHEN_Write_THEN_ShouldSerializeExpectedPayload()
        {
            var options = CreateOptions();
            var value = new TorrentMetadata(
                infoHashV1: "InfoHashV1",
                infoHashV2: "InfoHashV2",
                hash: "Hash",
                info: new TorrentMetadataInfo(
                    name: "Name",
                    length: 99,
                    pieceLength: 16,
                    piecesNum: 7,
                    @private: true,
                    files:
                    [
                        new TorrentMetadataFile(
                            path: "file.bin",
                            length: 99),
                    ]),
                trackers:
                [
                    new TorrentMetadataTracker(
                        url: "udp://tracker",
                        tier: 0),
                ],
                webSeeds: ["https://seed"],
                createdBy: "CreatedBy",
                creationDate: 946684800,
                comment: "Comment");

            var json = JsonSerializer.Serialize(value, options);

            json.Should().Contain("\"infohash_v1\":\"InfoHashV1\"");
            json.Should().Contain("\"infohash_v2\":\"InfoHashV2\"");
            json.Should().Contain("\"hash\":\"Hash\"");
            json.Should().Contain("\"created_by\":\"CreatedBy\"");
            json.Should().Contain("\"creation_date\":946684800");
            json.Should().Contain("\"comment\":\"Comment\"");
            json.Should().Contain("\"trackers\":[{\"url\":\"udp://tracker\",\"tier\":0}]");
            json.Should().Contain("\"webseeds\":[\"https://seed\"]");
            json.Should().Contain("\"info\":{\"name\":\"Name\",\"length\":99,\"piece_length\":16,\"pieces_num\":7,\"private\":true,\"files\":[{\"path\":\"file.bin\",\"length\":99}]}");
        }

        [Fact]
        public void GIVEN_MetadataWithoutOptionalFields_WHEN_Write_THEN_ShouldOmitNullableProperties()
        {
            var options = CreateOptions();
            var value = new TorrentMetadata(
                infoHashV1: null,
                infoHashV2: null,
                hash: null,
                info: new TorrentMetadataInfo(
                    name: "Name",
                    length: 99,
                    pieceLength: 16,
                    piecesNum: 7,
                    @private: false,
                    files: []),
                trackers: [],
                webSeeds: [],
                createdBy: null,
                creationDate: null,
                comment: null);

            var json = JsonSerializer.Serialize(value, options);

            json.Should().NotContain("infohash_v1");
            json.Should().NotContain("infohash_v2");
            json.Should().NotContain("\"hash\"");
            json.Should().NotContain("created_by");
            json.Should().NotContain("creation_date");
            json.Should().NotContain("\"comment\"");
            json.Should().Contain("\"trackers\":[]");
            json.Should().Contain("\"webseeds\":[]");
        }
    }
}
