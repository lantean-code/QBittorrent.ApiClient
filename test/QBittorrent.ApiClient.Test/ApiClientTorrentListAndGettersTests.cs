using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientTorrentListAndGettersTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTorrentListAndGettersTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_NoFilters_WHEN_GetTorrentList_THEN_ShouldGETWithoutQueryAndReturnList()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/info");
                req.RequestUri!.Query.Should().BeEmpty();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var result = (await _target.GetTorrentListAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_AllFilters_WHEN_GetTorrentList_THEN_ShouldIncludeAllParamsInOrder()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/info":
                        req.Method.Should().Be(HttpMethod.Get);
                        req.RequestUri!.Query.Should().Be("?filter=active&category=Movies&tag=HD&sort=name&reverse=true&limit=50&offset=5&hashes=a%7Cb%7Cc&private=true&includeFiles=false&includeTrackers=true");
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("[]")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.GetTorrentListAsync(
                filter: "active",
                category: "Movies",
                tag: "HD",
                sort: "name",
                reverse: true,
                limit: 50,
                offset: 5,
                isPrivate: true,
                includeFiles: false,
                includeTrackers: true,
                selector: TorrentSelector.FromHashes(["a", "b", "c"])
            , cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_BooleanFlagsFalseAndTrueMix_WHEN_GetTorrentList_THEN_ShouldSerializeExpectedBooleanValues()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.8"));

                    case "/torrents/info":
                        req.Method.Should().Be(HttpMethod.Get);
                        req.RequestUri!.Query.Should().Be("?private=false&includeFiles=true&includeTrackers=false");
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("[]")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.GetTorrentListAsync(
                isPrivate: false,
                includeFiles: true,
                includeTrackers: false, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTorrentListIncludeFilesAndIncludeFilesFalse_WHEN_GetTorrentList_THEN_ShouldOmitIncludeFilesQuery()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.7"));

                    case "/torrents/info":
                        req.Method.Should().Be(HttpMethod.Get);
                        req.RequestUri!.Query.Should().Be("?private=false&includeTrackers=false");
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("[]")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.GetTorrentListAsync(
                isPrivate: false,
                includeFiles: false,
                includeTrackers: false, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTorrentListIncludeFilesAndIncludeFilesTrue_WHEN_GetTorrentList_THEN_ShouldFailWithoutCallingTorrentEndpoint()
        {
            var torrentInfoRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.11.7"));

                    case "/torrents/info":
                        torrentInfoRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("[]")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetTorrentListAsync(includeFiles: true, cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.11.7 does not support including torrent file data in torrent list responses.");
            torrentInfoRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailureAndIncludeFiles_WHEN_GetTorrentList_THEN_ShouldReturnProbeFailure()
        {
            var torrentInfoRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/info":
                        torrentInfoRequestCount++;
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("[]")
                        });

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetTorrentListAsync(includeFiles: true, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.ServerError,
                statusCode: HttpStatusCode.BadGateway,
                userMessage: "probe failed");

            torrentInfoRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_TorrentPayload_WHEN_GetTorrentList_THEN_ShouldDeserializeTorrents()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/info");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "hash": "hash1",
                                "infohash_v1": "InfoHashV1",
                                "infohash_v2": "InfoHashV2",
                                "name": "Name",
                                "magnet_uri": "MagnetUri",
                                "size": 1000,
                                "progress": 0.5,
                                "dlspeed": 2,
                                "upspeed": 3,
                                "priority": 4,
                                "num_seeds": 5,
                                "num_complete": 6,
                                "num_leechs": 7,
                                "num_incomplete": 8,
                                "ratio": 1.2,
                                "popularity": 1.3,
                                "eta": 9,
                                "state": "downloading",
                                "seq_dl": true,
                                "f_l_piece_prio": false,
                                "category": "Movies",
                                "tags": "tag1,tag2",
                                "super_seeding": true,
                                "force_start": false,
                                "save_path": "/save",
                                "download_path": "/download",
                                "content_path": "/content",
                                "root_path": "/root",
                                "added_on": 10,
                                "completion_on": 11,
                                "tracker": "udp://tracker",
                                "trackers_count": 12,
                                "dl_limit": 13,
                                "up_limit": 14,
                                "downloaded": 15,
                                "uploaded": 16,
                                "downloaded_session": 17,
                                "uploaded_session": 18,
                                "amount_left": 19,
                                "completed": 20,
                                "connections_count": 21,
                                "connections_limit": 22,
                                "max_ratio": 1.4,
                                "max_seeding_time": 23,
                                "max_inactive_seeding_time": 24,
                                "ratio_limit": 1.5,
                                "seeding_time_limit": 25,
                                "inactive_seeding_time_limit": 26,
                                "share_limit_action": "Remove",
                                "seen_complete": 27,
                                "last_activity": 28,
                                "total_size": 29,
                                "auto_tmm": true,
                                "time_active": 30,
                                "seeding_time": 31,
                                "availability": 1.6,
                                "reannounce": 32,
                                "comment": "Comment",
                                "has_metadata": true,
                                "created_by": "Creator",
                                "creation_date": 33,
                                "private": true,
                                "total_wasted": 34,
                                "pieces_num": 35,
                                "piece_size": 36,
                                "pieces_have": 37,
                                "has_tracker_warning": true,
                                "has_tracker_error": false,
                                "has_other_announce_error": true,
                                "files":
                                [
                                    {
                                        "index": 1,
                                        "name": "FileName",
                                        "size": 38,
                                        "progress": 0.4,
                                        "priority": 1,
                                        "is_seed": true,
                                        "piece_range": [ 2, 3 ],
                                        "availability": 1.7
                                    }
                                ]
                            }
                        ]
                        """)
                });
            };

            var result = (await _target.GetTorrentListAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            var torrent = result[0];
            torrent.Hash.Should().Be("hash1");
            torrent.InfoHashV1.Should().Be("InfoHashV1");
            torrent.InfoHashV2.Should().Be("InfoHashV2");
            torrent.Name.Should().Be("Name");
            torrent.MagnetUri.Should().Be("MagnetUri");
            torrent.Size.Should().Be(1000);
            torrent.Progress.Should().Be(0.5);
            torrent.DownloadSpeed.Should().Be(2);
            torrent.UploadSpeed.Should().Be(3);
            torrent.Priority.Should().Be(4);
            torrent.NumberSeeds.Should().Be(5);
            torrent.NumberComplete.Should().Be(6);
            torrent.NumberLeeches.Should().Be(7);
            torrent.NumberIncomplete.Should().Be(8);
            torrent.Ratio.Should().Be(1.2);
            torrent.Popularity.Should().Be(1.3);
            torrent.EstimatedTimeOfArrival.Should().Be(9);
            torrent.State.Should().Be(TorrentState.Downloading);
            torrent.SequentialDownload.Should().BeTrue();
            torrent.FirstLastPiecePriority.Should().BeFalse();
            torrent.Category.Should().Be("Movies");
            torrent.Tags.Should().BeEquivalentTo(["tag1", "tag2"]);
            torrent.SuperSeeding.Should().BeTrue();
            torrent.ForceStart.Should().BeFalse();
            torrent.SavePath.Should().Be("/save");
            torrent.DownloadPath.Should().Be("/download");
            torrent.ContentPath.Should().Be("/content");
            torrent.RootPath.Should().Be("/root");
            torrent.AddedOn.Should().Be(10);
            torrent.CompletionOn.Should().Be(11);
            torrent.Tracker.Should().Be("udp://tracker");
            torrent.TrackersCount.Should().Be(12);
            torrent.DownloadLimit.Should().Be(13);
            torrent.UploadLimit.Should().Be(14);
            torrent.Downloaded.Should().Be(15);
            torrent.Uploaded.Should().Be(16);
            torrent.DownloadedSession.Should().Be(17);
            torrent.UploadedSession.Should().Be(18);
            torrent.AmountLeft.Should().Be(19);
            torrent.Completed.Should().Be(20);
            torrent.ConnectionsCount.Should().Be(21);
            torrent.ConnectionsLimit.Should().Be(22);
            torrent.MaxRatio.Should().Be(1.4);
            torrent.MaxSeedingTime.Should().Be(23);
            torrent.MaxInactiveSeedingTime.Should().Be(24);
            torrent.RatioLimit.Should().Be(1.5);
            torrent.SeedingTimeLimit.Should().Be(25);
            torrent.InactiveSeedingTimeLimit.Should().Be(26);
            torrent.ShareLimitAction.Should().Be(ShareLimitAction.Remove);
            torrent.SeenComplete.Should().Be(27);
            torrent.LastActivity.Should().Be(28);
            torrent.TotalSize.Should().Be(29);
            torrent.AutomaticTorrentManagement.Should().BeTrue();
            torrent.TimeActive.Should().Be(30);
            torrent.SeedingTime.Should().Be(31);
            torrent.Availability.Should().Be(1.6);
            torrent.Reannounce.Should().Be(32);
            torrent.Comment.Should().Be("Comment");
            torrent.HasMetadata.Should().BeTrue();
            torrent.CreatedBy.Should().Be("Creator");
            torrent.CreationDate.Should().Be(33);
            torrent.IsPrivate.Should().BeTrue();
            torrent.TotalWasted.Should().Be(34);
            torrent.PiecesCount.Should().Be(35);
            torrent.PieceSize.Should().Be(36);
            torrent.PiecesHave.Should().Be(37);
            torrent.HasTrackerWarning.Should().BeTrue();
            torrent.HasTrackerError.Should().BeFalse();
            torrent.HasOtherAnnounceError.Should().BeTrue();
            torrent.Files.Should().ContainSingle();
            torrent.Files![0].Index.Should().Be(1);
            torrent.Files[0].Name.Should().Be("FileName");
            torrent.Files[0].Size.Should().Be(38);
            torrent.Files[0].Progress.Should().Be(0.4);
            torrent.Files[0].Priority.Should().Be(Priority.Normal);
            torrent.Files[0].IsSeed.Should().BeTrue();
            torrent.Files[0].PieceRange.Should().BeEquivalentTo([2, 3]);
            torrent.Files[0].Availability.Should().Be(1.7);
        }

        [Fact]
        public async Task GIVEN_TorrentPayloadWithPreciseAndLargeValues_WHEN_GetTorrentList_THEN_ShouldPreserveRangeAndPrecision()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/info");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "hash": "hash1",
                                "progress": 0.1234567890123456,
                                "ratio": 1.234567890123456,
                                "popularity": 2.345678901234567,
                                "dl_limit": 2147483647,
                                "up_limit": 2147483646,
                                "max_ratio": 3.456789012345678,
                                "max_inactive_seeding_time": 321,
                                "ratio_limit": 4.567890123456789,
                                "inactive_seeding_time_limit": 654,
                                "time_active": 3000000000,
                                "availability": 5.67890123456789
                            }
                        ]
                        """)
                });
            };

            var result = (await _target.GetTorrentListAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            var torrent = result[0];
            torrent.Progress.Should().Be(0.1234567890123456);
            torrent.Ratio.Should().Be(1.234567890123456);
            torrent.Popularity.Should().Be(2.345678901234567);
            torrent.DownloadLimit.Should().Be(2147483647);
            torrent.UploadLimit.Should().Be(2147483646);
            torrent.MaxRatio.Should().Be(3.456789012345678);
            torrent.MaxInactiveSeedingTime.Should().Be(321);
            torrent.RatioLimit.Should().Be(4.567890123456789);
            torrent.InactiveSeedingTimeLimit.Should().Be(654);
            torrent.TimeActive.Should().Be(3000000000);
            torrent.Availability.Should().Be(5.67890123456789);
        }

        [Fact]
        public async Task GIVEN_Response_WHEN_GetTorrentCount_THEN_ShouldParseInteger()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("42")
            });

            var result = (await _target.GetTorrentCountAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Be(42);
        }

        [Fact]
        public async Task GIVEN_InvalidResponse_WHEN_GetTorrentCount_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("NaN")
            });

            var result = await _target.GetTorrentCountAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_BadJson_WHEN_GetTorrentList_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not json")
            });

            var result = await _target.GetTorrentListAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentList_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("no")
            });

            var result = await _target.GetTorrentListAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "no");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_GetTorrentProperties_THEN_ShouldGETWithHashAndDeserialize()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/properties?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            };

            var result = (await _target.GetTorrentPropertiesAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_TorrentPropertiesPayload_WHEN_GetTorrentProperties_THEN_ShouldMapFields()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/properties?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "addition_date": 1,
                            "comment": "Comment",
                            "completion_date": 2,
                            "created_by": "Creator",
                            "creation_date": 3,
                            "dl_limit": 4,
                            "dl_speed": 5,
                            "dl_speed_avg": 6,
                            "eta": 7,
                            "last_seen": 8,
                            "nb_connections": 9,
                            "nb_connections_limit": 10,
                            "peers": 11,
                            "peers_total": 12,
                            "piece_size": 13,
                            "pieces_have": 14,
                            "pieces_num": 15,
                            "reannounce": 16,
                            "save_path": "/save",
                            "download_path": "/download",
                            "seeding_time": 17,
                            "seeds": 18,
                            "seeds_total": 19,
                            "share_ratio": 1.1,
                            "popularity": 1.2,
                            "progress": 1.3,
                            "availability": 1.4,
                            "time_elapsed": 20,
                            "total_downloaded": 21,
                            "total_downloaded_session": 22,
                            "total_size": 23,
                            "total_uploaded": 24,
                            "total_uploaded_session": 25,
                            "total_wasted": 26,
                            "up_limit": 27,
                            "up_speed": 28,
                            "up_speed_avg": 29,
                            "infohash_v1": "InfoHashV1",
                            "infohash_v2": "InfoHashV2",
                            "hash": "hash1",
                            "name": "TorrentName",
                            "is_private": true,
                            "private": false,
                            "has_metadata": true
                        }
                        """)
                });
            };

            var result = (await _target.GetTorrentPropertiesAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.AdditionDate.Should().Be(1);
            result.Comment.Should().Be("Comment");
            result.CompletionDate.Should().Be(2);
            result.CreatedBy.Should().Be("Creator");
            result.CreationDate.Should().Be(3);
            result.DownloadLimit.Should().Be(4);
            result.DownloadSpeed.Should().Be(5);
            result.DownloadSpeedAverage.Should().Be(6);
            result.EstimatedTimeOfArrival.Should().Be(7);
            result.LastSeen.Should().Be(8);
            result.Connections.Should().Be(9);
            result.ConnectionsLimit.Should().Be(10);
            result.Peers.Should().Be(11);
            result.PeersTotal.Should().Be(12);
            result.PieceSize.Should().Be(13);
            result.PiecesHave.Should().Be(14);
            result.PiecesNum.Should().Be(15);
            result.Reannounce.Should().Be(16);
            result.SavePath.Should().Be("/save");
            result.DownloadPath.Should().Be("/download");
            result.SeedingTime.Should().Be(17);
            result.Seeds.Should().Be(18);
            result.SeedsTotal.Should().Be(19);
            result.ShareRatio.Should().Be(1.1);
            result.Popularity.Should().Be(1.2);
            result.Progress.Should().Be(1.3);
            result.Availability.Should().Be(1.4);
            result.TimeElapsed.Should().Be(20);
            result.TotalDownloaded.Should().Be(21);
            result.TotalDownloadedSession.Should().Be(22);
            result.TotalSize.Should().Be(23);
            result.TotalUploaded.Should().Be(24);
            result.TotalUploadedSession.Should().Be(25);
            result.TotalWasted.Should().Be(26);
            result.UploadLimit.Should().Be(27);
            result.UploadSpeed.Should().Be(28);
            result.UploadSpeedAverage.Should().Be(29);
            result.InfoHashV1.Should().Be("InfoHashV1");
            result.InfoHashV2.Should().Be("InfoHashV2");
            result.Hash.Should().Be("hash1");
            result.Name.Should().Be("TorrentName");
            result.IsPrivate.Should().BeTrue();
            result.Private.Should().BeFalse();
            result.HasMetadata.Should().BeTrue();
        }

        [Fact]
        public async Task GIVEN_TorrentPropertiesPayloadWithPreciseAndLargeValues_WHEN_GetTorrentProperties_THEN_ShouldPreserveRangeAndPrecision()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/properties?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "addition_date": 1,
                            "comment": "Comment",
                            "completion_date": 2,
                            "created_by": "Creator",
                            "creation_date": 3,
                            "dl_limit": 2147483647,
                            "dl_speed": 2147483646,
                            "dl_speed_avg": 6,
                            "eta": 7,
                            "last_seen": 8,
                            "nb_connections": 9,
                            "nb_connections_limit": 10,
                            "peers": 11,
                            "peers_total": 12,
                            "piece_size": 3000000000,
                            "pieces_have": 14,
                            "pieces_num": 15,
                            "reannounce": 3000000001,
                            "save_path": "/save",
                            "download_path": "/download",
                            "seeding_time": 3000000002,
                            "seeds": 18,
                            "seeds_total": 19,
                            "share_ratio": 1.1234567890123456,
                            "popularity": 2.1234567890123456,
                            "progress": 3.1234567890123456,
                            "availability": 4.1234567890123456,
                            "time_elapsed": 3000000003,
                            "total_downloaded": 21,
                            "total_downloaded_session": 22,
                            "total_size": 23,
                            "total_uploaded": 24,
                            "total_uploaded_session": 25,
                            "total_wasted": 26,
                            "up_limit": 2147483645,
                            "up_speed": 2147483644,
                            "up_speed_avg": 29,
                            "infohash_v1": "InfoHashV1",
                            "infohash_v2": "InfoHashV2"
                        }
                        """)
                });
            };

            var result = (await _target.GetTorrentPropertiesAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.DownloadLimit.Should().Be(2147483647);
            result.DownloadSpeed.Should().Be(2147483646);
            result.PieceSize.Should().Be(3000000000);
            result.Reannounce.Should().Be(3000000001);
            result.SeedingTime.Should().Be(3000000002);
            result.ShareRatio.Should().Be(1.1234567890123456);
            result.Popularity.Should().Be(2.1234567890123456);
            result.Progress.Should().Be(3.1234567890123456);
            result.Availability.Should().Be(4.1234567890123456);
            result.TimeElapsed.Should().Be(3000000003);
            result.UploadLimit.Should().Be(2147483645);
            result.UploadSpeed.Should().Be(2147483644);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentProperties_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.GetTorrentPropertiesAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_GetTorrentTrackers_THEN_ShouldGETAndReturnList()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/trackers?hash=xyz");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var result = (await _target.GetTorrentTrackersAsync("xyz", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_HashAndTrackerPayload_WHEN_GetTorrentTrackers_THEN_ShouldDeserializeTrackerAndEndpoints()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/trackers?hash=xyz");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "url": "Url",
                                "updating": true,
                                "status": 2,
                                "tier": 1,
                                "num_peers": 3,
                                "num_seeds": 4,
                                "num_leeches": 5,
                                "num_downloaded": 6,
                                "msg": "Message",
                                "next_announce": 7,
                                "min_announce": 8,
                                "endpoints": [
                                    {
                                        "name": "Name",
                                        "updating": true,
                                        "status": 3,
                                        "msg": "Message",
                                        "bt_version": 1,
                                        "num_peers": 2,
                                        "num_seeds": 3,
                                        "num_leeches": 4,
                                        "num_downloaded": 5,
                                        "next_announce": 6,
                                        "min_announce": 7
                                    }
                                ]
                            },
                            {
                                "url": "Url2",
                                "status": 1,
                                "tier": 2,
                                "num_peers": 8,
                                "num_seeds": 9,
                                "num_leeches": 10,
                                "num_downloaded": 11,
                                "msg": "Message2",
                                "next_announce": 12,
                                "min_announce": 13
                            }
                        ]
                        """)
                });
            };

            var result = (await _target.GetTorrentTrackersAsync("xyz", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().HaveCount(2);
            result[0].Url.Should().Be("Url");
            result[0].Updating.Should().BeTrue();
            result[0].Status.Should().Be(TrackerStatus.Working);
            result[0].Tier.Should().Be(1);
            result[0].Peers.Should().Be(3);
            result[0].Seeds.Should().Be(4);
            result[0].Leeches.Should().Be(5);
            result[0].Downloads.Should().Be(6);
            result[0].Message.Should().Be("Message");
            result[0].NextAnnounce.Should().Be(7);
            result[0].MinAnnounce.Should().Be(8);
            result[0].Endpoints.Should().ContainSingle();
            result[0].Endpoints[0].Name.Should().Be("Name");
            result[0].Endpoints[0].Updating.Should().BeTrue();
            result[0].Endpoints[0].Status.Should().Be(TrackerStatus.Updating);
            result[0].Endpoints[0].Message.Should().Be("Message");
            result[0].Endpoints[0].BitTorrentVersion.Should().Be(1);
            result[0].Endpoints[0].Peers.Should().Be(2);
            result[0].Endpoints[0].Seeds.Should().Be(3);
            result[0].Endpoints[0].Leeches.Should().Be(4);
            result[0].Endpoints[0].Downloads.Should().Be(5);
            result[0].Endpoints[0].NextAnnounce.Should().Be(6);
            result[0].Endpoints[0].MinAnnounce.Should().Be(7);

            result[1].Url.Should().Be("Url2");
            result[1].Updating.Should().BeNull();
            result[1].Status.Should().Be(TrackerStatus.Uncontacted);
            result[1].Tier.Should().Be(2);
            result[1].Peers.Should().Be(8);
            result[1].Seeds.Should().Be(9);
            result[1].Leeches.Should().Be(10);
            result[1].Downloads.Should().Be(11);
            result[1].Message.Should().Be("Message2");
            result[1].NextAnnounce.Should().Be(12);
            result[1].MinAnnounce.Should().Be(13);
            result[1].Endpoints.Should().BeEmpty();
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_GetTorrentWebSeeds_THEN_ShouldGETAndReturnList()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/webseeds?hash=h1");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var result = (await _target.GetTorrentWebSeedsAsync("h1", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_WebSeedsPayload_WHEN_GetTorrentWebSeeds_THEN_ShouldDeserializeSeeds()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/webseeds?hash=h1");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "url": "http://seed.example/file"
                            }
                        ]
                        """)
                });
            };

            var result = (await _target.GetTorrentWebSeedsAsync("h1", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            result[0].Url.Should().Be("http://seed.example/file");
        }

        [Fact]
        public async Task GIVEN_HashOnly_WHEN_GetTorrentContents_THEN_ShouldGETWithHashOnly()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/files");
                req.RequestUri!.Query.Should().Be("?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var result = (await _target.GetTorrentContentsAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_FileDataPayload_WHEN_GetTorrentContents_THEN_ShouldDeserializeContents()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/files");
                req.RequestUri!.Query.Should().Be("?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "index": 1,
                                "name": "FileName",
                                "size": 1000,
                                "progress": 0.5,
                                "priority": 7,
                                "is_seed": true,
                                "piece_range": [2, 5],
                                "availability": 1.2
                            }
                        ]
                        """)
                });
            };

            var result = (await _target.GetTorrentContentsAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            result[0].Index.Should().Be(1);
            result[0].Name.Should().Be("FileName");
            result[0].Size.Should().Be(1000);
            result[0].Progress.Should().Be(0.5);
            result[0].Priority.Should().Be((Priority)7);
            result[0].IsSeed.Should().BeTrue();
            result[0].PieceRange.Should().BeEquivalentTo([2, 5]);
            result[0].Availability.Should().Be(1.2);
        }

        [Fact]
        public async Task GIVEN_FileDataPayloadWithPreciseValues_WHEN_GetTorrentContents_THEN_ShouldPreservePrecision()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/files");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "index": 0,
                                "name": "file.mkv",
                                "size": 10,
                                "progress": 0.1234567890123456,
                                "priority": 1,
                                "is_seed": false,
                                "piece_range": [1, 2],
                                "availability": 1.234567890123456
                            }
                        ]
                        """)
                });
            };

            var result = (await _target.GetTorrentContentsAsync("hash", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            result[0].Progress.Should().Be(0.1234567890123456);
            result[0].Availability.Should().Be(1.234567890123456);
        }

        [Fact]
        public async Task GIVEN_FileDataWithNullPieceRange_WHEN_GetTorrentContents_THEN_ShouldUseEmptyPieceRange()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/files");
                req.RequestUri!.Query.Should().Be("?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "index": 2,
                                "name": "FileName2",
                                "size": 2000,
                                "progress": 1.0,
                                "priority": 1,
                                "is_seed": false,
                                "piece_range": null,
                                "availability": 2.0
                            }
                        ]
                        """)
                });
            };

            var result = (await _target.GetTorrentContentsAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().ContainSingle();
            result[0].PieceRange.Should().BeEmpty();
        }

        [Fact]
        public async Task GIVEN_Indexes_WHEN_GetTorrentContents_THEN_ShouldGETWithIndexesPipeSeparated()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrents/files");
                req.RequestUri!.Query.Should().Be("?hash=abc&indexes=1%7C2%7C3");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var result = (await _target.GetTorrentContentsAsync("abc", indexes: [1, 2, 3], cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_BadJson_WHEN_GetTorrentContents_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("oops")
            });

            var result = await _target.GetTorrentContentsAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentContents_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetTorrentContentsAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_GetTorrentPieceStates_THEN_ShouldGETAndReturnUnexpectedResponseOnBadJson()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/pieceStates?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("not json")
                });
            };

            var result = await _target.GetTorrentPieceStatesAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentPieceStates_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.GetTorrentPieceStatesAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_Hash_WHEN_GetTorrentPieceHashes_THEN_ShouldGETAndReturnStrings()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrents/pieceHashes?hash=abc");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[\"h1\",\"h2\"]")
                });
            };

            var result = (await _target.GetTorrentPieceHashesAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Should().Be("h1");
            result[1].Should().Be("h2");
        }

        [Fact]
        public async Task GIVEN_BadJson_WHEN_GetTorrentPieceHashes_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetTorrentPieceHashesAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentPieceHashes_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("err")
            });

            var result = await _target.GetTorrentPieceHashesAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "err");
        }

        [Fact]
        public async Task GIVEN_SupportedApiVersion_WHEN_GetTorrentPieceAvailability_THEN_ShouldGETAndReturnCounts()
        {
            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.1"));

                    case "/torrents/pieceAvailability":
                        req.Method.Should().Be(HttpMethod.Get);
                        req.RequestUri!.ToString().Should().Be("http://localhost/torrents/pieceAvailability?hash=abc");
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "[1,2,3]"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = (await _target.GetTorrentPieceAvailabilityAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Equal(1, 2, 3);
        }

        [Fact]
        public async Task GIVEN_ApiVersionBeforeTorrentPieceAvailability_WHEN_GetTorrentPieceAvailability_THEN_ShouldFailWithoutCallingEndpoint()
        {
            var availabilityRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "2.15.0"));

                    case "/torrents/pieceAvailability":
                        availabilityRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "[]"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetTorrentPieceAvailabilityAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(kind: ApiFailureKind.ValidationFailed);
            failure.UserMessage.Should().Be("qBittorrent Web API 2.15.0 does not support torrent piece availability.");
            availabilityRequestCount.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_ApiVersionProbeFailure_WHEN_GetTorrentPieceAvailability_THEN_ShouldReturnProbeFailure()
        {
            var availabilityRequestCount = 0;

            _handler.Responder = (req, _) =>
            {
                switch (req.RequestUri!.AbsolutePath)
                {
                    case "/app/webapiVersion":
                        return Task.FromResult(CreateResponse(HttpStatusCode.BadGateway, "probe failed"));

                    case "/torrents/pieceAvailability":
                        availabilityRequestCount++;
                        return Task.FromResult(CreateResponse(HttpStatusCode.OK, "[]"));

                    default:
                        throw new InvalidOperationException($"Unexpected request: {req.RequestUri}");
                }
            };

            var result = await _target.GetTorrentPieceAvailabilityAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.ServerError, statusCode: HttpStatusCode.BadGateway, userMessage: "probe failed");
            availabilityRequestCount.Should().Be(0);
        }

        private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string? content)
        {
            if (content is null)
            {
                return new HttpResponseMessage(statusCode);
            }

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            };
        }
    }
}
