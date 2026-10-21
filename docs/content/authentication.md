# Authentication and sessions

Choose username/password login with session cookies, or API-key authentication on a supported server. The focused examples below assume the client has already been registered.

## Session cookie behaviour

In a console application, `HttpClientHandler` stores the login cookie in a `CookieContainer` and sends it on later requests. Retain the same container when the factory replaces a handler. A container represents a shared session; use separate containers for independent accounts.

### Browser cookie behaviour

In WebAssembly, the browser stores and sends cookies. qbtmud's `CookieHandler` requests `BrowserRequestCredentials.Include`. `Include` allows credentialed cross-origin requests where permitted; it does not override CORS or cookie restrictions.

For separate UI and API origins, the API or proxy must allow the UI's specific origin and credentialed requests, and the cookie policy must permit them. Serving both through the same origin avoids that extra configuration. See Microsoft's [browser credential guidance](https://learn.microsoft.com/aspnet/core/blazor/call-web-api?view=aspnetcore-10.0#cookie-based-request-credentials).

Compatibility initialization does not authenticate a client or preserve cookies.

## Username and password

The following examples assume an `IApiClient` named `apiClient` is already registered and resolved.

```csharp
var loginResult = await apiClient.LoginAsync(username, password);
if (loginResult.IsFailure)
{
    Console.Error.WriteLine(loginResult.Failure.UserMessage);
    return;
}
```

Login establishes a cookie-backed WebUI session. Get `username` and `password` from configuration or secret storage in a console application, or from the user in a browser application. Compatibility initialization is a separate step after authentication.

## Check the session

```csharp
var authResult = await apiClient.CheckAuthStateAsync();
if (authResult.IsFailure)
{
    Console.Error.WriteLine(authResult.Failure.UserMessage);
    return;
}

var authenticated = authResult.Value;
Console.WriteLine(authenticated ? "Authenticated" : "Login required");
```

A successful check with a value of `false` means the session is not authenticated. A failed check means the client could not determine the state successfully. Handle those outcomes separately.

## Log out

```csharp
var logoutResult = await apiClient.LogoutAsync();
if (logoutResult.IsFailure)
{
    Console.Error.WriteLine(logoutResult.Failure.UserMessage);
    return;
}
```

## API-key authentication

qBittorrent supports API-key authentication from **5.2.0 / Web API 2.14.1**. Generate a key in qBittorrent's WebUI settings, then send it using `Authorization: Bearer <key>`. The [upstream API-key guide](https://github.com/qbittorrent/qBittorrent/wiki/API-Key-Authentication-%28%E2%89%A5v5.2.0%29) describes setup and supported requests.

When configuring your named HTTP client, set the authorization header from a key held in your application's secret storage:

```csharp
using System.Net.Http.Headers;

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
```

Here, `client` is the `HttpClient` passed to the `AddHttpClient` configuration delegate and `apiKey` is the existing key. Configure `UseCookies = false` on a console client's primary handler when using this option. The browser credential handler is not needed for bearer authentication.

The key authenticates each request. Skip `LoginAsync` and `LogoutAsync`: qBittorrent rejects the `auth/*` endpoints for API-key requests. Continue to initialize compatibility before using version-gated operations.

Use HTTPS when connecting over a network and keep the key in secret storage. For WebAssembly, the same authorization header can be configured on the named client, but obtain the key from the user at runtime; browser code and configuration are visible to users. Cross-origin calls must also allow the `Authorization` header. A bearer-key client does not need the cookie handler.

## Check API-key management support

On an already authenticated client, check the server capability before offering key-management actions:

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

if (!profile.SupportsApiKeyManagement)
{
    Console.WriteLine("This server does not support API-key management.");
    return;
}

Console.WriteLine("API-key management is available.");
```

The client exposes `RotateAPIKeyAsync()` and `DeleteAPIKeyAsync()` for supported servers. Rotation replaces the existing key, so update every consumer that uses it. Deletion revokes key-based access; use a cookie-authenticated session to configure a new key afterward. The examples above use an existing key and do not rotate or delete it.
