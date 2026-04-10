using System.Net;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace QBittorrent.ApiClient.Test
{
    public class ApiClientServiceCollectionExtensionsTests
    {
        [Fact]
        public void GIVEN_NullServices_WHEN_AddQBittorrentApiClientWithHttpClient_THEN_ShouldThrowArgumentNullException()
        {
            var httpClient = new HttpClient();

            var action = () => ApiClientServiceCollectionExtensions.AddQBittorrentApiClient(null!, httpClient);

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("services");
        }

        [Fact]
        public void GIVEN_NullHttpClient_WHEN_AddQBittorrentApiClientWithHttpClient_THEN_ShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            var action = () => services.AddQBittorrentApiClient((HttpClient)null!);

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("httpClient");
        }

        [Fact]
        public void GIVEN_NullConfigureAction_WHEN_AddQBittorrentApiClient_THEN_ShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            var action = () => services.AddQBittorrentApiClient((Action<HttpClient>)null!);

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("configureHttpClient");
        }

        [Fact]
        public void GIVEN_NullHttpClientName_WHEN_AddQBittorrentApiClient_THEN_ShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            var action = () => services.AddQBittorrentApiClient((string)null!);

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("httpClientName");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void GIVEN_EmptyHttpClientName_WHEN_AddQBittorrentApiClient_THEN_ShouldThrowArgumentException(string httpClientName)
        {
            var services = new ServiceCollection();

            var action = () => services.AddQBittorrentApiClient(httpClientName);

            action.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("httpClientName");
        }

        [Fact]
        public void GIVEN_NullServiceProviderConfigureAction_WHEN_AddQBittorrentApiClient_THEN_ShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            var action = () => services.AddQBittorrentApiClient((Action<IServiceProvider, HttpClient>)null!);

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("configureHttpClient");
        }

        [Fact]
        public async Task GIVEN_ProvidedHttpClient_WHEN_AddQBittorrentApiClient_THEN_ShouldRegisterTransientClientsThatShareCompatibilityCache()
        {
            var services = new ServiceCollection();
            var handler = new StubHttpMessageHandler();
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            var apiVersionRequestCount = 0;
            var loadRequestCount = 0;

            handler.Responder = (request, _) =>
            {
                return request.RequestUri!.AbsolutePath switch
                {
                    "/app/webapiVersion" => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent((++apiVersionRequestCount, "2.13.1").Item2)
                    }),
                    "/clientdata/load" => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent((++loadRequestCount, "{}").Item2)
                    }),
                    _ => throw new InvalidOperationException($"Unexpected request: {request.RequestUri}")
                };
            };

            services.AddQBittorrentApiClient(httpClient);

            using var serviceProvider = services.BuildServiceProvider();

            var firstClient = serviceProvider.GetRequiredService<IApiClient>();
            var secondClient = serviceProvider.GetRequiredService<IApiClient>();

            firstClient.Should().NotBeSameAs(secondClient);
            firstClient.Should().BeOfType<ApiClient>();
            secondClient.Should().BeOfType<ApiClient>();

            (await firstClient.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            (await secondClient.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            apiVersionRequestCount.Should().Be(1);
            loadRequestCount.Should().Be(2);
        }

        [Fact]
        public async Task GIVEN_NamedHttpClientRegistration_WHEN_AddQBittorrentApiClient_THEN_ShouldUseRegisteredClientAndShareCompatibilityCacheAcrossTransientClients()
        {
            var services = new ServiceCollection();
            var handler = new StubHttpMessageHandler();
            var apiVersionRequestCount = 0;
            var loadRequestCount = 0;

            handler.Responder = (request, _) =>
            {
                request.RequestUri!.Host.Should().Be("localhost");
                return request.RequestUri.AbsolutePath switch
                {
                    "/app/webapiVersion" => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent((++apiVersionRequestCount, "2.13.1").Item2)
                    }),
                    "/clientdata/load" => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent((++loadRequestCount, "{}").Item2)
                    }),
                    _ => throw new InvalidOperationException($"Unexpected request: {request.RequestUri}")
                };
            };

            services.AddHttpClient(
                "qbt",
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri("http://localhost/");
                })
                .ConfigurePrimaryHttpMessageHandler(() => handler);

            services.AddQBittorrentApiClient("qbt");

            using var serviceProvider = services.BuildServiceProvider();

            var firstClient = serviceProvider.GetRequiredService<IApiClient>();
            var secondClient = serviceProvider.GetRequiredService<IApiClient>();

            firstClient.Should().NotBeSameAs(secondClient);
            firstClient.Should().BeOfType<ApiClient>();
            secondClient.Should().BeOfType<ApiClient>();

            (await firstClient.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            (await secondClient.LoadClientDataAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldSucceed();
            apiVersionRequestCount.Should().Be(1);
            loadRequestCount.Should().Be(2);
        }

        [Fact]
        public void GIVEN_ConfigureAction_WHEN_AddQBittorrentApiClient_THEN_ShouldConfigureTransientClientPerResolution()
        {
            var services = new ServiceCollection();
            var configureCallCount = 0;

            services.AddQBittorrentApiClient(
                httpClient =>
                {
                    configureCallCount++;
                    httpClient.BaseAddress = new Uri("http://localhost/");
                });

            using var serviceProvider = services.BuildServiceProvider();

            var firstClient = serviceProvider.GetRequiredService<IApiClient>();
            var secondClient = serviceProvider.GetRequiredService<IApiClient>();

            firstClient.Should().NotBeSameAs(secondClient);
            firstClient.Should().BeOfType<ApiClient>();
            secondClient.Should().BeOfType<ApiClient>();
            configureCallCount.Should().Be(2);
        }

        [Fact]
        public void GIVEN_ServiceProviderConfigureAction_WHEN_AddQBittorrentApiClient_THEN_ShouldAllowDependencyDrivenConfigurationForTransientClients()
        {
            var services = new ServiceCollection();
            var configureCallCount = 0;

            services.AddSingleton(new Uri("http://localhost/"));
            services.AddQBittorrentApiClient(
                (serviceProvider, httpClient) =>
                {
                    configureCallCount++;
                    httpClient.BaseAddress = serviceProvider.GetRequiredService<Uri>();
                });

            using var serviceProvider = services.BuildServiceProvider();

            var firstClient = serviceProvider.GetRequiredService<IApiClient>();
            var secondClient = serviceProvider.GetRequiredService<IApiClient>();

            firstClient.Should().NotBeSameAs(secondClient);
            firstClient.Should().BeOfType<ApiClient>();
            secondClient.Should().BeOfType<ApiClient>();
            configureCallCount.Should().Be(2);
        }

        [Fact]
        public async Task GIVEN_MultipleConfigureRegistrations_WHEN_AddQBittorrentApiClient_THEN_ShouldKeepClientConfigurationsIsolated()
        {
            var services = new ServiceCollection();

            services.AddQBittorrentApiClient(
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri("http://localhost-a/");
                });

            services.AddQBittorrentApiClient(
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri("http://localhost-b/");
                });

            using var serviceProvider = services.BuildServiceProvider();

            var clients = serviceProvider.GetServices<IApiClient>().ToArray();
            clients.Should().HaveCount(2);

            var firstExportUrl = (await clients[0].GetExportUrlAsync("abc123")).GetValueOrThrow();
            var secondExportUrl = (await clients[1].GetExportUrlAsync("abc123")).GetValueOrThrow();

            firstExportUrl.Should().Be("http://localhost-a/torrents/export?hash=abc123");
            secondExportUrl.Should().Be("http://localhost-b/torrents/export?hash=abc123");
        }
    }
}
