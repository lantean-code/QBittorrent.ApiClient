# Version compatibility

The client handles request and response differences between qBittorrent Web API versions. The library supports qBittorrent **5.1.x and 5.2.x**.

## Initialize from the server

After configuring the client and authenticating:

```csharp
var result = await apiClient.InitializeAsync();
if (result.IsFailure)
{
    Console.Error.WriteLine(result.Failure.UserMessage);
    return;
}
```

`InitializeAsync()` calls `/app/webapiVersion` when no compatibility profile is available and builds the client's profile. Successful profiles are shared across resolved clients for the same configured base address in the service provider. Later initialization can reuse that profile without another HTTP probe. Failed or invalid initialization is not cached.

Initialize before compatibility-gated operations. Without a successful profile, those operations throw `InvalidOperationException`.

## Initialize from a known version

If your application already knows the server's **Web API version**, initialize without an HTTP probe:

```csharp
apiClient.Initialize(new Version(2, 13, 1));

if (!apiClient.Initialize("2.13.1"))
{
    Console.Error.WriteLine("The Web API version could not be initialized.");
    return;
}
```

These are alternative ways to initialize. Use the actual Web API version, such as `2.13.1`, rather than an application version such as `5.2.0`.

You can also supply the known version at registration:

```csharp
services.AddQBittorrentApiClient(httpClient, new Version(2, 13, 1));
```

The registration overloads also accept a version string. Use one registration appropriate for your application. Known-version initialization skips version discovery; it does not log in or enable features the server lacks.

## Read versions directly

`GetApplicationVersionAsync()` reads the qBittorrent application version. `GetAPIVersionAsync()` calls `/app/webapiVersion` directly on each invocation and returns that call's result.

Use `InitializeAsync()` to initialize client compatibility. Use `GetAPIVersionAsync()` when you need the raw Web API version.

## Check capabilities in your application

`WebApiCompatibilityProfile` provides capability checks outside the client's internal initialization state:

```csharp
var versionResult = await apiClient.GetAPIVersionAsync();
if (versionResult.IsFailure)
{
    Console.Error.WriteLine(versionResult.Failure.UserMessage);
    return;
}

if (!WebApiCompatibilityProfile.TryCreate(versionResult.Value, out var profile))
{
    Console.Error.WriteLine("The server returned an invalid Web API version.");
    return;
}

if (!profile.SupportsClientData)
{
    return;
}

var dataResult = await apiClient.LoadClientDataAsync();
if (dataResult.IsFailure)
{
    Console.Error.WriteLine(dataResult.Failure.UserMessage);
    return;
}

Console.WriteLine($"Stored keys: {dataResult.Value.Count}");
```

This example assumes the client has already been initialized. Creating a public profile does not initialize `IApiClient`.

Use the profile's capability properties to decide which features your application presents. The library keeps version-specific request shaping inside the client.
