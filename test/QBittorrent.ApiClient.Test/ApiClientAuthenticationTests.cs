using System.Net;
using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    public partial class ApiClientAuthenticationTests
    {
        private readonly ApiClient _target;
        private readonly StubHttpMessageHandler _handler;

        public ApiClientAuthenticationTests()
        {
            _handler = new StubHttpMessageHandler();
            var http = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _target = new ApiClient(http);
        }

        [Fact]
        public async Task GIVEN_ServerReturnsOK_WHEN_CheckAuthState_THEN_ShouldBeTrue()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri?.ToString().Should().Be("http://localhost/app/version");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            var result = (await _target.CheckAuthStateAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GIVEN_ServerReturnsUnauthorized_WHEN_CheckAuthState_THEN_ShouldBeFalse()
        {
            _handler.Responder = async (req, ct) =>
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            };

            var result = (await _target.CheckAuthStateAsync(cancellationToken: TestContext.Current.CancellationToken)).GetValueOrThrow();

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GIVEN_ServerReturnsServerError_WHEN_CheckAuthState_THEN_ShouldThrow()
        {
            _handler.Responder = async (req, ct) =>
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("ServerError")
                };
            };

            var result = await _target.CheckAuthStateAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "ServerError");
        }

        [Fact]
        public async Task GIVEN_HandlerThrows_WHEN_CheckAuthState_THEN_ShouldPropagateFailure()
        {
            _handler.Responder = (_, _) => throw new HttpRequestException("boom", null, HttpStatusCode.BadGateway);

            var result = await _target.CheckAuthStateAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(kind: ApiFailureKind.NoResponse, userMessage: "boom");
        }

        [Fact]
        public async Task GIVEN_ValidCredentialsAndSuccessStatus_WHEN_Login_THEN_ShouldPostFormAndNotThrow()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/auth/login");
                var body = await req.Content.ReadAsStringOrNullAsync(ct);
                body.Should().Be("username=user&password=pass");
                req.Content?.Headers.ContentType?.MediaType.Should().Be("application/x-www-form-urlencoded");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Ok")
                };
            };

            await _target.LoginAsync("user", "pass", cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_SuccessStatusButFailsBody_WHEN_Login_THEN_ShouldThrowBadRequest()
        {
            _handler.Responder = async (req, ct) =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Fails.")
                };
            };

            var result = await _target.LoginAsync("user", "pass", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.AuthenticationRejected,
                statusCode: HttpStatusCode.BadRequest,
                userMessage: "Invalid username or password.");
        }

        [Fact]
        public async Task GIVEN_NonSuccessStatus_WHEN_Login_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = async (req, ct) =>
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Nope")
                };
            };

            var result = await _target.LoginAsync("user", "pass", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.AuthenticationRejected,
                statusCode: HttpStatusCode.Unauthorized,
                userMessage: "Invalid username or password.");
        }

        [Fact]
        public async Task GIVEN_ForbiddenStatus_WHEN_Login_THEN_ShouldReturnBannedClientFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("banned")
            });

            var result = await _target.LoginAsync("user", "pass", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.AccessDenied,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "banned");

            failure.TryGetReason<LoginFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(LoginFailureReason.BannedClient);
        }

        [Fact]
        public async Task GIVEN_ForbiddenStatusWithoutBody_WHEN_Login_THEN_ShouldReturnDefaultBannedClientFailure()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden));

            var result = await _target.LoginAsync("user", "pass", cancellationToken: TestContext.Current.CancellationToken);

            var failure = result.ShouldFailWith(
                kind: ApiFailureKind.AccessDenied,
                statusCode: HttpStatusCode.Forbidden,
                userMessage: "The client has been temporarily banned from logging in.");

            failure.TryGetReason<LoginFailureReason>(out var reason).Should().BeTrue();
            reason.Should().Be(LoginFailureReason.BannedClient);
        }

        [Fact]
        public async Task GIVEN_UnexpectedStatus_WHEN_Login_THEN_ShouldUseGenericFailureMapping()
        {
            _handler.Responder = (_, _) => Task.FromResult(new HttpResponseMessage((HttpStatusCode)418)
            {
                Content = new StringContent("teapot")
            });

            var result = await _target.LoginAsync("user", "pass", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.UnexpectedResponse,
                statusCode: (HttpStatusCode)418,
                userMessage: "teapot");
        }

        [Fact]
        public async Task GIVEN_RequestException_WHEN_Login_THEN_ShouldReturnNoResponseFailure()
        {
            _handler.Responder = (_, _) => throw new HttpRequestException("login failed");

            var result = await _target.LoginAsync("user", "pass", cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(
                kind: ApiFailureKind.NoResponse,
                userMessage: "login failed");
        }

        [Fact]
        public async Task GIVEN_Success_WHEN_Logout_THEN_ShouldPostAndNotThrow()
        {
            _handler.Responder = async (req, ct) =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri?.ToString().Should().Be("http://localhost/auth/logout");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            await _target.LogoutAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task GIVEN_NonSuccess_WHEN_Logout_THEN_ShouldThrowWithStatusAndMessage()
        {
            _handler.Responder = async (req, ct) =>
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("fail")
                };
            };

            var result = await _target.LogoutAsync(cancellationToken: TestContext.Current.CancellationToken);

            result.ShouldFailWith(statusCode: HttpStatusCode.InternalServerError, userMessage: "fail");
        }
    }
}
