using AwesomeAssertions;

namespace QBittorrent.ApiClient.Test
{
    internal sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? Responder { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Responder.Should().NotBeNull();
            return Responder!(request, cancellationToken);
        }
    }
}
