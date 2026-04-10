using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientTorrentCreatorTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTorrentCreatorTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_NullRequest_WHEN_AddTorrentCreationTask_THEN_ShouldThrowArgumentNullException()
        {
            var act = async () => await _target.AddTorrentCreationTaskAsync(null!, cancellationToken: TestContext.Current.CancellationToken);

            var ex = await act.Should().ThrowAsync<ArgumentNullException>();
            ex.Which.ParamName.Should().Be("request");
        }

        [Fact]
        public async Task GIVEN_EmptySourcePath_WHEN_AddTorrentCreationTask_THEN_ShouldThrowArgumentException()
        {
            var act = async () => await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest
            {
                SourcePath = " "
            }, cancellationToken: TestContext.Current.CancellationToken);

            var ex = await act.Should().ThrowAsync<ArgumentException>();
            ex.Which.ParamName.Should().Be("request");
            ex.Which.Message.Should().Contain("SourcePath is required.");
        }

        [Fact]
        public async Task GIVEN_MinimalRequest_WHEN_AddTorrentCreationTask_THEN_ShouldPOSTOnlySourcePathAndReturnEmptyOnEmptyBody()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrentcreator/addTask");
                req.Content!.Headers.ContentType!.MediaType.Should().Be("application/x-www-form-urlencoded");

                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("sourcePath=%2Fsrc");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(string.Empty)
                };
            };

            var id = (await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest
            {
                SourcePath = "/src"
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be(string.Empty);
        }

        [Fact]
        public async Task GIVEN_AllFields_WHEN_AddTorrentCreationTask_THEN_ShouldIncludeEveryParameterAndReturnTaskId()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrentcreator/addTask");

                var form = await req.Content!.ReadAsStringAsync(ct);
                var parts = form.Split('&')
                    .Select(p => p.Split('='))
                    .ToDictionary(a => a[0], a => Uri.UnescapeDataString(a.Length > 1 ? a[1] : string.Empty));

                parts["sourcePath"].Should().Be("/src");
                parts["torrentFilePath"].Should().Be("/out.torrent");
                parts["pieceSize"].Should().Be("512");
                parts["private"].Should().Be("true");
                parts["startSeeding"].Should().Be("false");
                parts["comment"].Should().Be("hello");
                parts["source"].Should().Be("mysrc");
                parts["trackers"].Should().Be("t1|t2");
                parts["urlSeeds"].Should().Be("u1|u2");
                parts["format"].Should().Be("v2");
                parts["optimizeAlignment"].Should().Be("true");
                parts["paddedFileSizeLimit"].Should().Be("4096");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"taskID\":\"task-123\"}")
                };
            };

            var request = new TorrentCreationTaskRequest
            {
                SourcePath = "/src",
                TorrentFilePath = "/out.torrent",
                PieceSize = 512,
                Private = true,
                StartSeeding = false,
                Comment = "hello",
                Source = "mysrc",
                Trackers = new[] { "t1", "t2" },
                UrlSeeds = new[] { "u1", "u2" },
                Format = TorrentFormat.V2,
                OptimizeAlignment = true,
                PaddedFileSizeLimit = 4096
            };

            var id = (await _target.AddTorrentCreationTaskAsync(request, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be("task-123");
        }

        [Theory]
        [InlineData(TorrentFormat.V1, "v1")]
        [InlineData(TorrentFormat.Hybrid, "hybrid")]
        public async Task GIVEN_FormatVariant_WHEN_AddTorrentCreationTask_THEN_ShouldSerializeExpectedFormatValue(TorrentFormat format, string expectedValue)
        {
            _handler.Responder = async (req, ct) =>
            {
                var form = await req.Content!.ReadAsStringAsync(ct);
                var parts = form.Split('&')
                    .Select(p => p.Split('='))
                    .ToDictionary(a => a[0], a => Uri.UnescapeDataString(a.Length > 1 ? a[1] : string.Empty));

                parts.Should().ContainKey("sourcePath");
                parts["sourcePath"].Should().Be("/src");
                parts.Should().ContainKey("format");
                parts["format"].Should().Be(expectedValue);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"taskID\":\"task-123\"}")
                };
            };

            var id = (await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest
            {
                SourcePath = "/src",
                Format = format
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be("task-123");
        }

        [Fact]
        public async Task GIVEN_OKButNoTaskIdInJson_WHEN_AddTorrentCreationTask_THEN_ShouldReturnEmptyString()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            var id = (await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest
            {
                SourcePath = "/src"
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be(string.Empty);
        }

        [Fact]
        public async Task GIVEN_OKButNullTaskId_WHEN_AddTorrentCreationTask_THEN_ShouldReturnEmptyString()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"taskID\":null}")
            });

            var id = (await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest
            {
                SourcePath = "/src"
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be(string.Empty);
        }

        [Fact]
        public async Task GIVEN_OKButNullPayload_WHEN_AddTorrentCreationTask_THEN_ShouldReturnEmptyString()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            });

            var id = (await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest
            {
                SourcePath = "/src"
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be(string.Empty);
        }

        [Fact]
        public async Task GIVEN_OKButArrayPayload_WHEN_AddTorrentCreationTask_THEN_ShouldReturnEmptyString()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            });

            var id = (await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest
            {
                SourcePath = "/src"
            }, cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            id.Should().Be(string.Empty);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_AddTorrentCreationTask_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad req")
            });

            var result = await _target.AddTorrentCreationTaskAsync(new TorrentCreationTaskRequest { SourcePath = "/src" }, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad req");
        }

        [Fact]
        public async Task GIVEN_NoTaskId_WHEN_GetTorrentCreationTasks_THEN_ShouldGETWithoutQueryAndReturnList()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.AbsolutePath.Should().Be("/torrentcreator/status");
                req.RequestUri!.Query.Should().BeEmpty();

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var list = (await _target.GetTorrentCreationTasksAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            list.Should().NotBeNull();
            list.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_TaskId_WHEN_GetTorrentCreationTasks_THEN_ShouldGETWithQuery()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrentcreator/status?taskID=task-1");

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]")
                });
            };

            var list = (await _target.GetTorrentCreationTasksAsync("task-1", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            list.Should().NotBeNull();
            list.Count.Should().Be(0);
        }

        [Fact]
        public async Task GIVEN_TorrentCreationTaskStatusPayload_WHEN_GetTorrentCreationTasks_THEN_ShouldDeserializeTasks()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrentcreator/status");

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "taskID": "TaskId",
                                "sourcePath": "/source",
                                "pieceSize": 512,
                                "private": true,
                                "timeAdded": "2024-01-01 00:00",
                                "format": "v2",
                                "optimizeAlignment": false,
                                "paddedFileSizeLimit": 4096,
                                "status": "Running",
                                "comment": "Comment",
                                "torrentFilePath": "/output.torrent",
                                "source": "Source",
                                "trackers": [ "t1", "t2" ],
                                "urlSeeds": [ "u1", "u2" ],
                                "timeStarted": "2024-01-01 00:01",
                                "timeFinished": "2024-01-01 00:02",
                                "errorMessage": "ErrorMessage",
                                "progress": 75
                            }
                        ]
                        """)
                });
            };

            var list = (await _target.GetTorrentCreationTasksAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            list.Should().ContainSingle();
            list[0].TaskId.Should().Be("TaskId");
            list[0].SourcePath.Should().Be("/source");
            list[0].PieceSize.Should().Be(512);
            list[0].Private.Should().BeTrue();
            list[0].TimeAdded.Should().Be("2024-01-01 00:00");
            list[0].Format.Should().Be(TorrentFormat.V2);
            list[0].OptimizeAlignment.Should().BeFalse();
            list[0].PaddedFileSizeLimit.Should().Be(4096);
            list[0].Status.Should().Be(TorrentCreationTaskStatusKind.Running);
            list[0].Comment.Should().Be("Comment");
            list[0].TorrentFilePath.Should().Be("/output.torrent");
            list[0].Source.Should().Be("Source");
            list[0].Trackers.Should().BeEquivalentTo(new[] { "t1", "t2" });
            list[0].UrlSeeds.Should().BeEquivalentTo(new[] { "u1", "u2" });
            list[0].TimeStarted.Should().Be("2024-01-01 00:01");
            list[0].TimeFinished.Should().Be("2024-01-01 00:02");
            list[0].ErrorMessage.Should().Be("ErrorMessage");
            list[0].Progress.Should().Be(75);
        }

        [Fact]
        public async Task GIVEN_TorrentCreationTaskStatusWithNullLists_WHEN_GetTorrentCreationTasks_THEN_ShouldUseEmptyLists()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrentcreator/status");

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "taskID": "TaskId",
                                "trackers": null,
                                "urlSeeds": null
                            }
                        ]
                        """)
                });
            };

            var list = (await _target.GetTorrentCreationTasksAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            list.Should().ContainSingle();
            list[0].Trackers.Should().BeEmpty();
            list[0].UrlSeeds.Should().BeEmpty();
        }

        [Fact]
        public async Task GIVEN_BadJson_WHEN_GetTorrentCreationTasks_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("oops")
            });

            var result = await _target.GetTorrentCreationTasksAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentCreationTasks_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("busy")
            });

            var result = await _target.GetTorrentCreationTasksAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.ServiceUnavailable, userMessage: "busy");
        }

        [Fact]
        public async Task GIVEN_TaskId_WHEN_GetTorrentCreationTaskFile_THEN_ShouldGETWithQueryAndReturnBytes()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrentcreator/torrentFile?taskID=abc");

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(new byte[] { 1, 2 })
                });
            };

            var bytes = (await _target.GetTorrentCreationTaskFileAsync("abc", cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            bytes.Should().Equal(new byte[] { 1, 2 });
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetTorrentCreationTaskFile_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            });

            var result = await _target.GetTorrentCreationTaskFileAsync("x", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.NotFound, userMessage: "missing");
        }

        [Fact]
        public async Task GIVEN_TaskId_WHEN_DeleteTorrentCreationTask_THEN_ShouldPOSTForm()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().Be("http://localhost/torrentcreator/deleteTask");
                req.Content!.Headers.ContentType!.MediaType.Should().Be("application/x-www-form-urlencoded");

                var body = await req.Content!.ReadAsStringAsync(ct);
                body.Should().Be("taskID=abc");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.DeleteTorrentCreationTaskAsync("abc", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_DeleteTorrentCreationTask_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("nope")
            });

            var result = await _target.DeleteTorrentCreationTaskAsync("abc", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "nope");
        }
    }
}
