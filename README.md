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

Register the client through dependency injection, resolve `IApiClient`, authenticate, then initialize compatibility metadata before using version-gated operations:

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

var initializeResult = await apiClient.InitializeAsync();
if (!initializeResult.IsSuccess)
{
    return;
}

var versionResult = await apiClient.GetApplicationVersionAsync();
if (versionResult.TryGetValue(out var version))
{
    Console.WriteLine(version);
}
```

## Compatibility Initialization

Some qBittorrent Web API operations require version-specific request or response handling. Call `InitializeAsync()` once after configuring the client, and before using compatibility-gated operations.

`InitializeAsync()` calls qBittorrent's `/app/webapiVersion` endpoint, builds an internal compatibility profile, and shares that profile across resolved `IApiClient` instances for the same configured base address. Failed or invalid initialization is not cached.

If the qBittorrent Web API version is already known, initialize without making an HTTP call:

```csharp
apiClient.Initialize(new Version(2, 13, 1));

if (!apiClient.Initialize("2.13.1"))
{
    return;
}
```

Dependency-injection registrations also support known-version initialization with either a `Version` or a version string:

```csharp
services.AddQBittorrentApiClient(
    httpClient =>
    {
        httpClient.BaseAddress = new Uri("http://localhost:8080/api/v2/");
    },
    new Version(2, 13, 1));

services.AddQBittorrentApiClient(
    httpClient =>
    {
        httpClient.BaseAddress = new Uri("http://localhost:8080/api/v2/");
    },
    "2.13.1");
```

Calling a compatibility-gated operation before successful initialization is a developer usage error and throws `InvalidOperationException`.

`GetAPIVersionAsync()` remains the direct qBittorrent Web API call for `/app/webapiVersion`. It always calls the endpoint and returns that API call's result.

If you need compatibility checks outside `IApiClient`, use `WebApiCompatibilityProfile` with the returned Web API version:

```csharp
var apiVersionResult = await apiClient.GetAPIVersionAsync();
if (!apiVersionResult.TryGetValue(out var rawApiVersion) ||
    !WebApiCompatibilityProfile.TryCreate(rawApiVersion, out var compatibilityProfile))
{
    return;
}

if (compatibilityProfile.SupportsClientData)
{
    var clientDataResult = await apiClient.LoadClientDataAsync();
}
```

## Scope

- qBittorrent Web API client operations
- Structured result and failure handling
- Version-aware request shaping for supported qBittorrent releases

## Notes

- The concrete `ApiClient` implementation is internal; consume the package through `IApiClient`.
- Configure authentication, cookies, and base address on the supplied `HttpClient`.
- Call `InitializeAsync()` after configuring/authenticating the client, or use a known-version initialization overload, before calling compatibility-gated operations.
- Session persistence depends on the `HttpClient` and handler pipeline that you supply. Use your own handler configuration if you want cookie-backed authenticated sessions.
- The package does not include app-specific UI or hosting behavior.
