# Console examples

These complete `Program.cs` examples show registration, authentication, compatibility initialization, and a first request in a .NET 10 console application. Add the package as described in [Getting started](../getting-started.md#install). Choose one example for your application.

## Username and password

Set `QBITTORRENT_USERNAME` and `QBITTORRENT_PASSWORD` in your environment. Replace the API base address with your qBittorrent instance's address.

```csharp
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using QBittorrent.ApiClient;

var username = Environment.GetEnvironmentVariable("QBITTORRENT_USERNAME");
var password = Environment.GetEnvironmentVariable("QBITTORRENT_PASSWORD");
if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
{
    Console.Error.WriteLine("Set QBITTORRENT_USERNAME and QBITTORRENT_PASSWORD.");
    return;
}

var services = new ServiceCollection();
var cookies = new CookieContainer();
services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:8080/api/v2/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = true,
    CookieContainer = cookies
});
services.AddQBittorrentApiClient("API");
using var serviceProvider = services.BuildServiceProvider();
var apiClient = serviceProvider.GetRequiredService<IApiClient>();

var loginResult = await apiClient.LoginAsync(username, password);
if (loginResult.IsFailure)
{
    Console.Error.WriteLine(loginResult.Failure.UserMessage);
    return;
}

var initializeResult = await apiClient.InitializeAsync();
if (initializeResult.IsFailure)
{
    Console.Error.WriteLine(initializeResult.Failure.UserMessage);
    return;
}

var versionResult = await apiClient.GetApplicationVersionAsync();
if (versionResult.IsFailure)
{
    Console.Error.WriteLine(versionResult.Failure.UserMessage);
    return;
}

Console.WriteLine($"Connected to qBittorrent {versionResult.Value}");
```

The cookie container represents one account and is retained when the HTTP factory replaces a handler. Keep the service provider alive for the application's lifetime.

## API key

Use this alternative on qBittorrent 5.2.0 / Web API 2.14.1 or later. Set `QBITTORRENT_API_KEY` to an existing key generated in qBittorrent's WebUI settings. Replace the API base address with your server's address.

```csharp
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using QBittorrent.ApiClient;

var apiKey = Environment.GetEnvironmentVariable("QBITTORRENT_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine("Set QBITTORRENT_API_KEY.");
    return;
}

var services = new ServiceCollection();
services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:8080/api/v2/");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = false
});
services.AddQBittorrentApiClient("API");
using var serviceProvider = services.BuildServiceProvider();
var apiClient = serviceProvider.GetRequiredService<IApiClient>();

var initializeResult = await apiClient.InitializeAsync();
if (initializeResult.IsFailure)
{
    Console.Error.WriteLine(initializeResult.Failure.UserMessage);
    return;
}

var versionResult = await apiClient.GetApplicationVersionAsync();
if (versionResult.IsFailure)
{
    Console.Error.WriteLine(versionResult.Failure.UserMessage);
    return;
}

Console.WriteLine($"Connected to qBittorrent {versionResult.Value}");
```

The bearer key authenticates each request, so this example does not call `LoginAsync` or `LogoutAsync`. See [Authentication and sessions](../authentication.md) for authentication behaviour and key management.
