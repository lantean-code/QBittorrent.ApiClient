# Getting started

Add the library to your application, configure its HTTP client, and register `IApiClient`. This guide covers console and Blazor WebAssembly applications targeting .NET 10, with qBittorrent 5.1.x, 5.2.x, or 5.3.x.

## Install

Add the package to your application:

```bash
dotnet add package QBittorrent.ApiClient
```

Use a released version appropriate for your application. These docs follow `develop`; see [release notes](release-notes.md) for package changes.

## Configure the API address

Set the HTTP client's `BaseAddress` to your qBittorrent instance's API root, including `/api/v2/` and the trailing slash, for example `http://localhost:8080/api/v2/`. Include any reverse-proxy path prefix. The library sends relative endpoint paths under this address.

## Register in a console application

Create a service collection, configure a named HTTP client, then connect the library to that name. This registration prepares a cookie-backed session:

```csharp
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using QBittorrent.ApiClient;

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
```

The factory manages HTTP clients and handlers. Creating the cookie container outside the handler factory preserves it when a handler is replaced. Use separate containers for independent accounts.

Build the provider and resolve the client where your console application needs it:

```csharp
using var serviceProvider = services.BuildServiceProvider();
var apiClient = serviceProvider.GetRequiredService<IApiClient>();
```

Keep the provider alive for the application's lifetime. In an application that already uses the .NET host, add these registrations to its existing service collection instead of creating another provider.

## Register in Blazor WebAssembly

In an existing WebAssembly application's `Program.cs`, register the named client before building the host. Keep your existing root component registrations. This example assumes the UI and API share a base address, as in qbtmud's standard hosting setup:

```csharp
using Microsoft.Extensions.DependencyInjection;
using QBittorrent.ApiClient;

var apiBaseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), "api/v2/");

builder.Services.AddTransient<CookieHandler>();
builder.Services.AddHttpClient("API", client => client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<CookieHandler>();
builder.Services.AddQBittorrentApiClient("API");
```

Add `CookieHandler.cs` to the client project, using your application's namespace if needed:

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Http;

/// <summary>
/// Includes browser credentials on qBittorrent API requests.
/// </summary>
public sealed class CookieHandler : DelegatingHandler
{
    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}
```

This is qbtmud's browser credential pattern. The browser manages cookies; the delegating handler tells it to include credentials. The HTTP registration uses `Microsoft.Extensions.Http`, provided by the library's dependencies. The browser extension comes from `Microsoft.AspNetCore.Components.WebAssembly`, normally referenced by the WebAssembly client project.

Inject `IApiClient` into your component or service. If the API is hosted elsewhere, set `apiBaseAddress` to its absolute API root. [Authentication and sessions](authentication.md#browser-cookie-behaviour) explains browser credential and cross-origin behaviour.

## Choose authentication

Choose username/password login with session cookies, or an API key on a supported server. [Authentication and sessions](authentication.md) covers each option with focused examples. For API-key authentication, configure the named client's authorization header instead of cookie handling.

After authentication is ready, call `InitializeAsync()` before compatibility-gated operations. Known-version initialization is also available; see [Version compatibility](compatibility.md).

Consume the package through `IApiClient`. The concrete `ApiClient` implementation is internal.

## Application examples

The child pages show how these pieces work together:

- [Console examples](examples/console.md): complete username/password and API-key programs.
- [WebAssembly example](examples/webassembly.md): a component that connects using user-entered credentials.

For individual operations, see [Common operations](common-operations.md).
