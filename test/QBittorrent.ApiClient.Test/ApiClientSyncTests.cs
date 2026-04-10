using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientSyncTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientSyncTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_RequestId_WHEN_GetMainData_THEN_ShouldGETWithRidAndDeserialize()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/sync/maindata?rid=123");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "rid": 123,
                            "full_update": true,
                            "torrents":
                            {
                                "hash1":
                                {
                                    "hash": "hash1",
                                    "name": "TorrentName",
                                    "size": 12345,
                                    "progress": 0.5,
                                    "dlspeed": 10,
                                    "upspeed": 11,
                                    "priority": 2,
                                    "num_seeds": 3,
                                    "num_complete": 4,
                                    "num_leechs": 5,
                                    "num_incomplete": 6,
                                    "ratio": 0.7,
                                    "popularity": 0.8,
                                    "eta": 120,
                                    "state": "downloading",
                                    "category": "Movies",
                                    "tags": "tag1,tag2",
                                    "save_path": "/downloads",
                                    "download_path": "/downloads/incomplete",
                                    "content_path": "/downloads/content",
                                    "root_path": "/downloads/root",
                                    "added_on": 1,
                                    "completion_on": 2,
                                    "tracker": "udp://tracker",
                                    "trackers_count": 1,
                                    "dl_limit": 100,
                                    "up_limit": 101,
                                    "downloaded": 102,
                                    "uploaded": 103,
                                    "downloaded_session": 104,
                                    "uploaded_session": 105,
                                    "amount_left": 106,
                                    "completed": 107,
                                    "connections_count": 108,
                                    "connections_limit": 109,
                                    "max_ratio": 1.5,
                                    "max_seeding_time": 110,
                                    "max_inactive_seeding_time": 111,
                                    "ratio_limit": 1.7,
                                    "seeding_time_limit": 112,
                                    "inactive_seeding_time_limit": 113,
                                    "share_limit_action": "Remove",
                                    "seen_complete": 114,
                                    "last_activity": 115,
                                    "total_size": 116,
                                    "auto_tmm": true,
                                    "time_active": 117,
                                    "seeding_time": 118,
                                    "availability": 0.9,
                                    "reannounce": 119,
                                    "comment": "Comment",
                                    "has_metadata": true,
                                    "created_by": "Creator",
                                    "creation_date": 120,
                                    "private": false,
                                    "total_wasted": 121,
                                    "pieces_num": 122,
                                    "piece_size": 123,
                                    "pieces_have": 124,
                                    "has_tracker_warning": true,
                                    "has_tracker_error": false,
                                    "has_other_announce_error": true,
                                    "trackers":
                                    [
                                        {
                                            "url": "udp://tracker",
                                            "status": 2,
                                            "tier": 1,
                                            "num_peers": 3,
                                            "num_seeds": 4,
                                            "num_leeches": 5,
                                            "num_downloaded": 6,
                                            "msg": "Message",
                                            "next_announce": 7,
                                            "min_announce": 8
                                        }
                                    ]
                                }
                            },
                            "torrents_removed": [ "hash-removed" ],
                            "categories":
                            {
                                "Movies":
                                {
                                    "name": "Movies",
                                    "savePath": "/downloads/movies"
                                }
                            },
                            "categories_removed": [ "OldCategory" ],
                            "tags": [ "tag1", "tag2" ],
                            "tags_removed": [ "oldTag" ],
                            "trackers":
                            {
                                "udp://tracker": [ "hash1" ]
                            },
                            "trackers_removed": [ "udp://removed" ],
                            "server_state":
                            {
                                "connection_status": "connected",
                                "dht_nodes": 12,
                                "dl_info_data": 13,
                                "dl_info_speed": 14,
                                "dl_rate_limit": 15,
                                "up_info_data": 16,
                                "up_info_speed": 17,
                                "up_rate_limit": 18,
                                "last_external_address_v4": "1.2.3.4",
                                "last_external_address_v6": "::1",
                                "alltime_dl": 1000,
                                "alltime_ul": 1001,
                                "average_time_queue": 4,
                                "free_space_on_disk": 2000,
                                "global_ratio": 1.3,
                                "queued_io_jobs": 5,
                                "queueing": true,
                                "read_cache_hits": 0.4,
                                "read_cache_overload": 0.5,
                                "refresh_interval": 1500,
                                "total_buffers_size": 50,
                                "total_peer_connections": 51,
                                "total_queued_size": 52,
                                "total_wasted_session": 53,
                                "use_alt_speed_limits": false,
                                "use_subcategories": true,
                                "write_cache_overload": 0.6
                            }
                        }
                        """)
                });
            };

            var result = (await _target.GetMainDataAsync(123, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.ResponseId.Should().Be(123);
            result.FullUpdate.Should().BeTrue();
            result.Torrents.Should().NotBeNull();
            result.Torrents.Should().ContainKey("hash1");
            result.TorrentsRemoved.Should().BeEquivalentTo(new[] { "hash-removed" });
            result.Categories.Should().ContainKey("Movies");
            result.CategoriesRemoved.Should().BeEquivalentTo(new[] { "OldCategory" });
            result.Tags.Should().BeEquivalentTo(new[] { "tag1", "tag2" });
            result.TagsRemoved.Should().BeEquivalentTo(new[] { "oldTag" });
            result.Trackers.Should().ContainKey("udp://tracker");
            result.TrackersRemoved.Should().BeEquivalentTo(new[] { "udp://removed" });

            var torrent = result.Torrents["hash1"];
            torrent.Hash.Should().Be("hash1");
            torrent.Name.Should().Be("TorrentName");
            torrent.Size.Should().Be(12345);
            torrent.Progress.Should().Be(0.5);
            torrent.DownloadSpeed.Should().Be(10);
            torrent.UploadSpeed.Should().Be(11);
            torrent.Priority.Should().Be(2);
            torrent.NumberSeeds.Should().Be(3);
            torrent.NumberComplete.Should().Be(4);
            torrent.NumberLeeches.Should().Be(5);
            torrent.NumberIncomplete.Should().Be(6);
            torrent.Ratio.Should().Be(0.7);
            torrent.Popularity.Should().Be(0.8);
            torrent.EstimatedTimeOfArrival.Should().Be(120);
            torrent.State.Should().Be(TorrentState.Downloading);
            torrent.Category.Should().Be("Movies");
            torrent.Tags.Should().BeEquivalentTo(new[] { "tag1", "tag2" });
            torrent.SavePath.Should().Be("/downloads");
            torrent.DownloadPath.Should().Be("/downloads/incomplete");
            torrent.ContentPath.Should().Be("/downloads/content");
            torrent.RootPath.Should().Be("/downloads/root");
            torrent.AddedOn.Should().Be(1);
            torrent.CompletionOn.Should().Be(2);
            torrent.Tracker.Should().Be("udp://tracker");
            torrent.TrackersCount.Should().Be(1);
            torrent.DownloadLimit.Should().Be(100);
            torrent.UploadLimit.Should().Be(101);
            torrent.Downloaded.Should().Be(102);
            torrent.Uploaded.Should().Be(103);
            torrent.DownloadedSession.Should().Be(104);
            torrent.UploadedSession.Should().Be(105);
            torrent.AmountLeft.Should().Be(106);
            torrent.Completed.Should().Be(107);
            torrent.ConnectionsCount.Should().Be(108);
            torrent.ConnectionsLimit.Should().Be(109);
            torrent.MaxRatio.Should().Be(1.5);
            torrent.MaxSeedingTime.Should().Be(110);
            torrent.MaxInactiveSeedingTime.Should().Be(111);
            torrent.RatioLimit.Should().Be(1.7);
            torrent.SeedingTimeLimit.Should().Be(112);
            torrent.InactiveSeedingTimeLimit.Should().Be(113);
            torrent.ShareLimitAction.Should().Be(ShareLimitAction.Remove);
            torrent.SeenComplete.Should().Be(114);
            torrent.LastActivity.Should().Be(115);
            torrent.TotalSize.Should().Be(116);
            torrent.AutomaticTorrentManagement.Should().BeTrue();
            torrent.TimeActive.Should().Be(117);
            torrent.SeedingTime.Should().Be(118);
            torrent.Availability.Should().Be(0.9);
            torrent.Reannounce.Should().Be(119);
            torrent.Comment.Should().Be("Comment");
            torrent.HasMetadata.Should().BeTrue();
            torrent.CreatedBy.Should().Be("Creator");
            torrent.CreationDate.Should().Be(120);
            torrent.IsPrivate.Should().BeFalse();
            torrent.TotalWasted.Should().Be(121);
            torrent.PiecesCount.Should().Be(122);
            torrent.PieceSize.Should().Be(123);
            torrent.PiecesHave.Should().Be(124);
            torrent.HasTrackerWarning.Should().BeTrue();
            torrent.HasTrackerError.Should().BeFalse();
            torrent.HasOtherAnnounceError.Should().BeTrue();
            torrent.Trackers.Should().ContainSingle();
            torrent.Trackers![0].Url.Should().Be("udp://tracker");

            result.ServerState.Should().NotBeNull();
            result.ServerState!.ConnectionStatus.Should().Be(ConnectionStatus.Connected);
            result.ServerState.DHTNodes.Should().Be(12);
            result.ServerState.DownloadInfoData.Should().Be(13);
            result.ServerState.DownloadInfoSpeed.Should().Be(14);
            result.ServerState.DownloadRateLimit.Should().Be(15);
            result.ServerState.UploadInfoData.Should().Be(16);
            result.ServerState.UploadInfoSpeed.Should().Be(17);
            result.ServerState.UploadRateLimit.Should().Be(18);
            result.ServerState.LastExternalAddressV4.Should().Be("1.2.3.4");
            result.ServerState.LastExternalAddressV6.Should().Be("::1");
            result.ServerState.AllTimeDownloaded.Should().Be(1000);
            result.ServerState.AllTimeUploaded.Should().Be(1001);
            result.ServerState.AverageTimeQueue.Should().Be(4);
            result.ServerState.FreeSpaceOnDisk.Should().Be(2000);
            result.ServerState.GlobalRatio.Should().Be(1.3);
            result.ServerState.QueuedIOJobs.Should().Be(5);
            result.ServerState.Queuing.Should().BeTrue();
            result.ServerState.ReadCacheHits.Should().Be(0.4);
            result.ServerState.ReadCacheOverload.Should().Be(0.5);
            result.ServerState.RefreshInterval.Should().Be(1500);
            result.ServerState.TotalBuffersSize.Should().Be(50);
            result.ServerState.TotalPeerConnections.Should().Be(51);
            result.ServerState.TotalQueuedSize.Should().Be(52);
            result.ServerState.TotalWastedSession.Should().Be(53);
            result.ServerState.UseAltSpeedLimits.Should().BeFalse();
            result.ServerState.UseSubcategories.Should().BeTrue();
            result.ServerState.WriteCacheOverload.Should().Be(0.6);
        }

        [Fact]
        public async Task GIVEN_MainDataWithStringRatiosAndLargeCounters_WHEN_GetMainData_THEN_ShouldPreserveRangeAndParseStrings()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/sync/maindata?rid=1");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "rid": 1,
                            "server_state":
                            {
                                "connection_status": "connected",
                                "dht_nodes": 3000000000,
                                "dl_info_data": 13,
                                "dl_info_speed": 14,
                                "dl_rate_limit": 2147483647,
                                "up_info_data": 16,
                                "up_info_speed": 17,
                                "up_rate_limit": 2147483646,
                                "alltime_dl": 1000,
                                "alltime_ul": 1001,
                                "average_time_queue": 3000000001,
                                "free_space_on_disk": 2000,
                                "global_ratio": "1.23456789012345",
                                "queued_io_jobs": 3000000002,
                                "queueing": true,
                                "read_cache_hits": "2.34567890123456",
                                "read_cache_overload": "-",
                                "refresh_interval": 1500,
                                "total_buffers_size": 3000000003,
                                "total_peer_connections": 3000000004,
                                "total_queued_size": 3000000005,
                                "total_wasted_session": 53,
                                "use_alt_speed_limits": false,
                                "use_subcategories": true,
                                "write_cache_overload": "3.45678901234567"
                            }
                        }
                        """)
                });
            };

            var result = (await _target.GetMainDataAsync(1, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.ServerState.Should().NotBeNull();
            result.ServerState!.DHTNodes.Should().Be(3000000000);
            result.ServerState.DownloadRateLimit.Should().Be(2147483647);
            result.ServerState.UploadRateLimit.Should().Be(2147483646);
            result.ServerState.AverageTimeQueue.Should().Be(3000000001);
            result.ServerState.GlobalRatio.Should().Be(1.23456789012345);
            result.ServerState.QueuedIOJobs.Should().Be(3000000002);
            result.ServerState.ReadCacheHits.Should().Be(2.34567890123456);
            result.ServerState.ReadCacheOverload.Should().BeNull();
            result.ServerState.TotalBuffersSize.Should().Be(3000000003);
            result.ServerState.TotalPeerConnections.Should().Be(3000000004);
            result.ServerState.TotalQueuedSize.Should().Be(3000000005);
            result.ServerState.WriteCacheOverload.Should().Be(3.45678901234567);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetMainData_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetMainDataAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadGateway, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_NullJsonBody_WHEN_GetMainData_THEN_ShouldThrowInvalidOperation()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            });

            var result = await _target.GetMainDataAsync(5, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse);
        }

        [Fact]
        public async Task GIVEN_HashAndRid_WHEN_GetTorrentPeersData_THEN_ShouldGETWithParamsAndDeserialize()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/sync/torrentPeers?hash=abcdef&rid=7");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "full_update": true,
                            "peers":
                            {
                                "peer1":
                                {
                                    "client": "Client",
                                    "connection": "BT",
                                    "country": "Country",
                                    "country_code": "US",
                                    "dl_speed": 100,
                                    "downloaded": 101,
                                    "files": "Files",
                                    "flags": "Flags",
                                    "flags_desc": "FlagsDescription",
                                    "host_name": "HostName",
                                    "ip": "127.0.0.1",
                                    "i2p_dest": "Destination",
                                    "peer_id_client": "ClientId",
                                    "port": 6881,
                                    "progress": 0.5,
                                    "relevance": 0.7,
                                    "up_speed": 200,
                                    "uploaded": 201
                                }
                            },
                            "peers_removed": [ "peer-old" ],
                            "rid": 7,
                            "show_flags": true
                        }
                        """)
                });
            };

            var result = (await _target.GetTorrentPeersDataAsync("abcdef", 7, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.FullUpdate.Should().BeTrue();
            result.RequestId.Should().Be(7);
            result.ShowFlags.Should().BeTrue();
            result.PeersRemoved.Should().BeEquivalentTo(new[] { "peer-old" });
            result.Peers.Should().ContainKey("peer1");
            var peer = result.Peers!["peer1"];
            peer.Client.Should().Be("Client");
            peer.Connection.Should().Be(PeerConnectionType.Bittorrent);
            peer.Country.Should().Be("Country");
            peer.CountryCode.Should().Be("US");
            peer.DownloadSpeed.Should().Be(100);
            peer.Downloaded.Should().Be(101);
            peer.Files.Should().Be("Files");
            peer.Flags.Should().Be("Flags");
            peer.FlagsDescription.Should().Be("FlagsDescription");
            peer.HostName.Should().Be("HostName");
            peer.IPAddress.Should().Be("127.0.0.1");
            peer.I2pDestination.Should().Be("Destination");
            peer.ClientId.Should().Be("ClientId");
            peer.Port.Should().Be(6881);
            peer.Progress.Should().Be(0.5);
            peer.Relevance.Should().Be(0.7);
            peer.UploadSpeed.Should().Be(200);
            peer.Uploaded.Should().Be(201);
        }

        [Fact]
        public async Task GIVEN_TorrentPeersDataWithPreciseValues_WHEN_GetTorrentPeersData_THEN_ShouldPreservePrecision()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/sync/torrentPeers?hash=abcdef&rid=8");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        {
                            "peers":
                            {
                                "peer1":
                                {
                                    "dl_speed": 2147483647,
                                    "downloaded": 101,
                                    "progress": 0.1234567890123456,
                                    "relevance": 0.2345678901234567,
                                    "up_speed": 2147483646,
                                    "uploaded": 201
                                }
                            },
                            "rid": 8
                        }
                        """)
                });
            };

            var result = (await _target.GetTorrentPeersDataAsync("abcdef", 8, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            var peer = result.Peers!["peer1"];
            peer.DownloadSpeed.Should().Be(2147483647);
            peer.Progress.Should().Be(0.1234567890123456);
            peer.Relevance.Should().Be(0.2345678901234567);
            peer.UploadSpeed.Should().Be(2147483646);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentPeersData_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.GetTorrentPeersDataAsync("abc", 1, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_NullJsonBody_WHEN_GetTorrentPeersData_THEN_ShouldThrowInvalidOperation()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            });

            var result = await _target.GetTorrentPeersDataAsync("abc", 1, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse);
        }
    }
}
