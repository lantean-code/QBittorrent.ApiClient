# QBittorrent.ApiClient

`QBittorrent.ApiClient` is a .NET client library for the qBittorrent Web API. It provides typed operations, structured results and failures, and version-aware request and response handling.

The package targets .NET 10. The library supports qBittorrent **5.1.x, 5.2.x, and 5.3.x**.

!!! note "Documentation version"
    This site follows the `develop` branch. Examples describe the current source and may include changes that are not yet available in the latest NuGet package. Check the [release notes](release-notes.md) when choosing a package version.

## Start using the library

The [getting started guide](getting-started.md) explains how to install the package and configure `IApiClient` in your application. Its child pages provide complete [console examples](examples/console.md) and a [WebAssembly example](examples/webassembly.md).

```bash
dotnet add package QBittorrent.ApiClient
```

## Guides

- [Getting started](getting-started.md): add and configure the client in your application.
- [Console examples](examples/console.md) and [WebAssembly example](examples/webassembly.md): connect and make your first request.
- [Authentication and sessions](authentication.md): configure cookies, check authentication, and log out.
- [Results and failures](results-and-failures.md): read successful values, handle pending operations, and inspect failures.
- [Version compatibility](compatibility.md): initialize the client and check server capabilities.
- [Common operations](common-operations.md): list torrents, select torrents, start and stop them, and add a torrent.

## Project links

- [Source and issues](https://github.com/lantean-code/QBittorrent.ApiClient)
- [NuGet package](https://www.nuget.org/packages/QBittorrent.ApiClient)
- [Release notes](release-notes.md)

The library handles Web API calls. Your application supplies the HTTP configuration, credentials, session lifetime, and any polling or retry policy it needs.
