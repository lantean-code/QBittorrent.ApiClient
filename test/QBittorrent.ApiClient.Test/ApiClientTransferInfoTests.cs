using System.Net;
using AwesomeAssertions;
using QBittorrent.ApiClient.Models;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientTransferInfoTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientTransferInfoTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_OKJson_WHEN_GetGlobalTransferStatistics_THEN_ShouldDeserialize()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            var result = (await _target.GetGlobalTransferStatisticsAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GIVEN_GlobalTransferStatisticsPayload_WHEN_GetGlobalTransferStatistics_THEN_ShouldMapAllFields()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                        "connection_status": "connected",
                        "dht_nodes": 10,
                        "dl_info_data": 11,
                        "dl_info_speed": 12,
                        "dl_rate_limit": 13,
                        "up_info_data": 14,
                        "up_info_speed": 15,
                        "up_rate_limit": 16,
                        "last_external_address_v4": "1.2.3.4",
                        "last_external_address_v6": "::1"
                    }
                    """)
            });

            var result = (await _target.GetGlobalTransferStatisticsAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.ConnectionStatus.Should().Be(ConnectionStatus.Connected);
            result.DHTNodes.Should().Be(10);
            result.DownloadInfoData.Should().Be(11);
            result.DownloadInfoSpeed.Should().Be(12);
            result.DownloadRateLimit.Should().Be(13);
            result.UploadInfoData.Should().Be(14);
            result.UploadInfoSpeed.Should().Be(15);
            result.UploadRateLimit.Should().Be(16);
            result.LastExternalAddressV4.Should().Be("1.2.3.4");
            result.LastExternalAddressV6.Should().Be("::1");
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetGlobalTransferStatistics_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.GetGlobalTransferStatisticsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadGateway, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_ResponseIsOne_WHEN_GetAlternativeSpeedLimitsState_THEN_ShouldBeTrue()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("1")
            });

            var result = (await _target.GetAlternativeSpeedLimitsStateAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GIVEN_ResponseIsZero_WHEN_GetAlternativeSpeedLimitsState_THEN_ShouldBeFalse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("0")
            });

            var result = (await _target.GetAlternativeSpeedLimitsStateAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_GetAlternativeSpeedLimitsState_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("no")
            });

            var result = await _target.GetAlternativeSpeedLimitsStateAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Unauthorized, userMessage: "no");
        }

        [Fact]
        public async Task GIVEN_InvalidFlag_WHEN_GetAlternativeSpeedLimitsState_THEN_ShouldReturnUnexpectedResponse()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("2")
            });

            var result = await _target.GetAlternativeSpeedLimitsStateAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                userMessage: "qBittorrent returned an unexpected response.");
        }

        [Fact]
        public async Task GIVEN_Value_WHEN_SetAlternativeSpeedLimitsState_THEN_ShouldPOSTMode()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/transfer/setSpeedLimitsMode");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("mode=1");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetAlternativeSpeedLimitsStateAsync(true, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_FalseValue_WHEN_SetAlternativeSpeedLimitsState_THEN_ShouldPOSTModeZero()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.RequestUri?.ToString().Should().Be("http://localhost/transfer/setSpeedLimitsMode");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("mode=0");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetAlternativeSpeedLimitsStateAsync(false, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_OK_WHEN_ToggleAlternativeSpeedLimits_THEN_ShouldPOSTAndNotThrow()
        {
            _handler.Responder = (req, _) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/transfer/toggleSpeedLimitsMode");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            };

            await _target.ToggleAlternativeSpeedLimitsAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_ToggleAlternativeSpeedLimits_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("err")
            });

            var result = await _target.ToggleAlternativeSpeedLimitsAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "err");
        }

        [Fact]
        public async Task GIVEN_Digits_WHEN_GetGlobalDownloadLimit_THEN_ShouldParseLong()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("1234567890")
            });

            var result = (await _target.GetGlobalDownloadLimitAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Be(1234567890);
        }

        [Fact]
        public async Task GIVEN_InvalidNumber_WHEN_GetGlobalDownloadLimit_THEN_ShouldThrowFormatException()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("oops")
            });

            var result = await _target.GetGlobalDownloadLimitAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse);
        }

        [Fact]
        public async Task GIVEN_Limit_WHEN_SetGlobalDownloadLimit_THEN_ShouldPOSTFormWithLimit()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/transfer/setDownloadLimit");
                req.Content?.Headers.ContentType?.MediaType.Should().Be("application/x-www-form-urlencoded");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("limit=5000");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetGlobalDownloadLimitAsync(5000, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetGlobalDownloadLimit_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad")
            });

            var result = await _target.SetGlobalDownloadLimitAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.BadRequest, userMessage: "bad");
        }

        [Fact]
        public async Task GIVEN_Digits_WHEN_GetGlobalUploadLimit_THEN_ShouldParseLong()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("4321")
            });

            var result = (await _target.GetGlobalUploadLimitAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().Be(4321);
        }

        [Fact]
        public async Task GIVEN_InvalidNumber_WHEN_GetGlobalUploadLimit_THEN_ShouldThrowFormatException()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("NaN")
            });

            var result = await _target.GetGlobalUploadLimitAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.UnexpectedResponse);
        }

        [Fact]
        public async Task GIVEN_Limit_WHEN_SetGlobalUploadLimit_THEN_ShouldPOSTFormWithLimit()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/transfer/setUploadLimit");
                req.Content?.Headers.ContentType?.MediaType.Should().Be("application/x-www-form-urlencoded");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("limit=9001");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.SetGlobalUploadLimitAsync(9001, cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_SetGlobalUploadLimit_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("nope")
            });

            var result = await _target.SetGlobalUploadLimitAsync(1, cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Forbidden, userMessage: "nope");
        }

        [Fact]
        public async Task GIVEN_EmptyPeers_WHEN_BanPeers_THEN_ShouldPOSTFormWithEmptyPeersValue()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/transfer/banPeers");
                req.Content?.Headers.ContentType?.MediaType.Should().Be("application/x-www-form-urlencoded");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("peers=");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.BanPeersAsync([], cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_BanPeers_THEN_ShouldThrow()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("conflict")
            });

            var result = await _target.BanPeersAsync([], cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.Conflict, userMessage: "conflict");
        }
    }
}
