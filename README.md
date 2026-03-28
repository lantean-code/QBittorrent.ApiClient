# QBittorrent.ApiClient

`QBittorrent.ApiClient` is a .NET client library for the qBittorrent Web API.

It provides a result-based client surface, structured API failures, and version-aware compatibility handling for qBittorrent Web API differences such as the 5.1.x and 5.2.x lines.

## qBittorrent Support
 - 5.1.x
 - 5.2.x

## Installation

```bash
dotnet add package QBittorrent.ApiClient
```

## Usage

Register the client through dependency injection and resolve `IApiClient`:

```csharp
using QBittorrent.ApiClient;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddQBittorrentApiClient(
    httpClient =>
    {
        httpClient.BaseAddress = new Uri("http://localhost:8080/api/v2/");
    });

using var serviceProvider = services.BuildServiceProvider();

var apiClient = serviceProvider.GetRequiredService<IApiClient>();

var loginResult = await apiClient.LoginAsync("admin", "password");
if (!loginResult.IsSuccess)
{
    return;
}

var versionResult = await apiClient.GetApplicationVersionAsync();
if (versionResult.TryGetValue(out var version))
{
    Console.WriteLine(version);
}
```

## Compatibility Cache

Version-sensitive operations cache the qBittorrent Web API version internally and reuse that compatibility information across resolved `IApiClient` instances for the same configured base address. If the target server changes or is upgraded, refresh that cached compatibility profile explicitly:

```csharp
await apiClient.RefreshCompatibilityAsync();
```

## Scope

- qBittorrent Web API client operations
- Structured result and failure handling
- Version-aware request shaping for supported qBittorrent releases

## Notes

- The concrete `ApiClient` implementation is internal; consume the package through `IApiClient`.
- Configure authentication, cookies, and base address on the supplied `HttpClient`.
- Session persistence depends on the `HttpClient` and handler pipeline that you supply. Use your own handler configuration if you want cookie-backed authenticated sessions.
- The package does not include app-specific UI or hosting behavior.
