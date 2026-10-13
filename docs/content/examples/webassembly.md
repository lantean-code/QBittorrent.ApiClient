# WebAssembly example

This component connects to qBittorrent using credentials entered by the user, initializes compatibility, and displays the application version. It uses the `IApiClient` and browser cookie handler registered in [Getting started](../getting-started.md#register-in-blazor-webassembly).

Add `Connect.razor` to your WebAssembly client project:

```razor
@using QBittorrent.ApiClient
@inject IApiClient ApiClient

<label>
    Username
    <input autocomplete="username" @bind="_username" />
</label>
<label>
    Password
    <input type="password" autocomplete="current-password" @bind="_password" />
</label>
<button type="button" disabled="@_connecting" @onclick="ConnectAsync">Connect</button>
<p role="status">@_message</p>

@code
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _message = string.Empty;
    private bool _connecting;

    private async Task ConnectAsync()
    {
        _connecting = true;
        _message = string.Empty;

        try
        {
            var loginResult = await ApiClient.LoginAsync(_username, _password);
            _password = string.Empty;
            if (loginResult.IsFailure)
            {
                _message = loginResult.Failure.UserMessage;
                return;
            }

            var initializeResult = await ApiClient.InitializeAsync();
            if (initializeResult.IsFailure)
            {
                _message = initializeResult.Failure.UserMessage;
                return;
            }

            var versionResult = await ApiClient.GetApplicationVersionAsync();
            if (versionResult.IsFailure)
            {
                _message = versionResult.Failure.UserMessage;
                return;
            }

            _message = $"Connected to qBittorrent {versionResult.Value}";
        }
        finally
        {
            _password = string.Empty;
            _connecting = false;
        }
    }
}
```

Render the component with `<Connect />` from a page in the same namespace, or import its namespace first. The connect button stays disabled while requests are in progress. Each failure ends the sequence and displays the server operation's message.

The browser keeps the session cookie for later calls. Do not embed credentials in browser code or downloaded configuration. [Authentication and sessions](../authentication.md) explains session checks, logout, API keys, and cross-origin behaviour.
