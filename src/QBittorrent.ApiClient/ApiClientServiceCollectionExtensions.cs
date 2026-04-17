using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Provides dependency-injection registration helpers for <see cref="IApiClient" />.
    /// </summary>
    public static class ApiClientServiceCollectionExtensions
    {
        private const string _httpClientName = "QBittorrent.ApiClient";
        private static long _httpClientNameSequence;

        /// <summary>
        /// Registers qBittorrent API clients that use the provided <see cref="HttpClient" /> instance.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="httpClient">The preconfigured HTTP client to use for qBittorrent API calls.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(httpClient);

            services.TryAddSingleton<ApiClientCompatibilityProfileCache>();
            services.AddTransient<IApiClient>(
                serviceProvider => new ApiClient(httpClient, serviceProvider.GetRequiredService<ApiClientCompatibilityProfileCache>()));

            return services;
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients that use the provided <see cref="HttpClient" /> instance.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="httpClient">The preconfigured HTTP client to use for qBittorrent API calls.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            HttpClient httpClient,
            Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(httpClient);

            var compatibilityProfile = CreateCompatibilityProfile(webApiVersion);
            services.TryAddSingleton<ApiClientCompatibilityProfileCache>();
            services.AddTransient<IApiClient>(
                serviceProvider => new ApiClient(httpClient, serviceProvider.GetRequiredService<ApiClientCompatibilityProfileCache>(), compatibilityProfile));

            return services;
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients that use the provided <see cref="HttpClient" /> instance.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="httpClient">The preconfigured HTTP client to use for qBittorrent API calls.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version string.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            HttpClient httpClient,
            string webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(httpClient);

            var compatibilityProfile = CreateCompatibilityProfile(webApiVersion);
            services.TryAddSingleton<ApiClientCompatibilityProfileCache>();
            services.AddTransient<IApiClient>(
                serviceProvider => new ApiClient(httpClient, serviceProvider.GetRequiredService<ApiClientCompatibilityProfileCache>(), compatibilityProfile));

            return services;
        }

        /// <summary>
        /// Registers qBittorrent API clients and configures their internal <see cref="HttpClient" /> instances.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHttpClient">Configures the HTTP client used for qBittorrent API calls.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            Action<HttpClient> configureHttpClient)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureHttpClient);

            return services.AddQBittorrentApiClient((_, httpClient) => configureHttpClient(httpClient));
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients and configures their internal <see cref="HttpClient" /> instances.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHttpClient">Configures the HTTP client used for qBittorrent API calls.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            Action<HttpClient> configureHttpClient,
            Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureHttpClient);

            return services.AddQBittorrentApiClient((_, httpClient) => configureHttpClient(httpClient), webApiVersion);
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients and configures their internal <see cref="HttpClient" /> instances.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHttpClient">Configures the HTTP client used for qBittorrent API calls.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version string.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            Action<HttpClient> configureHttpClient,
            string webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureHttpClient);

            return services.AddQBittorrentApiClient((_, httpClient) => configureHttpClient(httpClient), webApiVersion);
        }

        /// <summary>
        /// Registers qBittorrent API clients that use a named <see cref="HttpClient" /> registration already configured by the caller.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="httpClientName">The name of the preconfigured HTTP client registration.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            string httpClientName)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(httpClientName);

            services.TryAddSingleton<ApiClientCompatibilityProfileCache>();
            services.AddTransient<IApiClient>(
                serviceProvider =>
                {
                    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                    var compatibilityProfileCache = serviceProvider.GetRequiredService<ApiClientCompatibilityProfileCache>();
                    return new ApiClient(httpClientFactory.CreateClient(httpClientName), compatibilityProfileCache);
                });

            return services;
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients that use a named <see cref="HttpClient" /> registration already configured by the caller.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="httpClientName">The name of the preconfigured HTTP client registration.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            string httpClientName,
            Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(httpClientName);

            var compatibilityProfile = CreateCompatibilityProfile(webApiVersion);
            services.TryAddSingleton<ApiClientCompatibilityProfileCache>();
            services.AddTransient<IApiClient>(
                serviceProvider =>
                {
                    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                    var compatibilityProfileCache = serviceProvider.GetRequiredService<ApiClientCompatibilityProfileCache>();
                    return new ApiClient(httpClientFactory.CreateClient(httpClientName), compatibilityProfileCache, compatibilityProfile);
                });

            return services;
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients that use a named <see cref="HttpClient" /> registration already configured by the caller.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="httpClientName">The name of the preconfigured HTTP client registration.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version string.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            string httpClientName,
            string webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(httpClientName);

            var compatibilityProfile = CreateCompatibilityProfile(webApiVersion);
            services.TryAddSingleton<ApiClientCompatibilityProfileCache>();
            services.AddTransient<IApiClient>(
                serviceProvider =>
                {
                    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                    var compatibilityProfileCache = serviceProvider.GetRequiredService<ApiClientCompatibilityProfileCache>();
                    return new ApiClient(httpClientFactory.CreateClient(httpClientName), compatibilityProfileCache, compatibilityProfile);
                });

            return services;
        }

        /// <summary>
        /// Registers qBittorrent API clients and configures their internal <see cref="HttpClient" /> instances using the service provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHttpClient">Configures the HTTP client used for qBittorrent API calls.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            Action<IServiceProvider, HttpClient> configureHttpClient)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureHttpClient);

            var httpClientName = CreateHttpClientRegistrationName();

            services.AddHttpClient(httpClientName, configureHttpClient);
            return services.AddQBittorrentApiClient(httpClientName);
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients and configures their internal <see cref="HttpClient" /> instances using the service provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHttpClient">Configures the HTTP client used for qBittorrent API calls.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            Action<IServiceProvider, HttpClient> configureHttpClient,
            Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureHttpClient);

            var httpClientName = CreateHttpClientRegistrationName();

            services.AddHttpClient(httpClientName, configureHttpClient);
            return services.AddQBittorrentApiClient(httpClientName, webApiVersion);
        }

        /// <summary>
        /// Registers initialized qBittorrent API clients and configures their internal <see cref="HttpClient" /> instances using the service provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHttpClient">Configures the HTTP client used for qBittorrent API calls.</param>
        /// <param name="webApiVersion">The known qBittorrent Web API version string.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddQBittorrentApiClient(
            this IServiceCollection services,
            Action<IServiceProvider, HttpClient> configureHttpClient,
            string webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureHttpClient);

            var httpClientName = CreateHttpClientRegistrationName();

            services.AddHttpClient(httpClientName, configureHttpClient);
            return services.AddQBittorrentApiClient(httpClientName, webApiVersion);
        }

        private static ApiClientCompatibilityProfile CreateCompatibilityProfile(Version webApiVersion)
        {
            ArgumentNullException.ThrowIfNull(webApiVersion);

            return new ApiClientCompatibilityProfile(webApiVersion);
        }

        private static ApiClientCompatibilityProfile CreateCompatibilityProfile(string webApiVersion)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(webApiVersion);

            var normalizedWebApiVersion = webApiVersion.Trim();
            if (!ApiClientCompatibilityProfile.TryCreate(normalizedWebApiVersion, out var compatibilityProfile))
            {
                throw new ArgumentException("The Web API version must be a valid version string.", nameof(webApiVersion));
            }

            return compatibilityProfile;
        }

        private static string CreateHttpClientRegistrationName()
        {
            var sequence = Interlocked.Increment(ref _httpClientNameSequence);
            return $"{_httpClientName}.{sequence}";
        }
    }
}
