# Common operations

These examples assume an authenticated, initialized `IApiClient` named `apiClient`, as shown in the [console examples](examples/console.md). Import the models namespace for selectors and request objects:

```csharp
using QBittorrent.ApiClient.Models;
```

## List torrents

```csharp
var result = await apiClient.GetTorrentListAsync(sort: "name", limit: 20);
if (result.IsFailure)
{
    Console.Error.WriteLine(result.Failure.UserMessage);
    return;
}

foreach (var torrent in result.Value)
{
    Console.WriteLine($"{torrent.Hash}: {torrent.Name}");
}
```

Optional arguments include category, tag, filter, sorting, and pagination. Feature availability can depend on the Web API version; see [version compatibility](compatibility.md).

## Select torrents

Build a selector from hashes returned by the torrent list:

```csharp
var selector = TorrentSelector.FromHash(hash);
```

For several hashes, use `TorrentSelector.FromHashes(hashes)`. For an intentional operation on every torrent, use `TorrentSelector.AllTorrents()`.

## Stop or start a torrent

```csharp
var result = await apiClient.StopTorrentsAsync(TorrentSelector.FromHash(hash));
if (result.IsFailure)
{
    Console.Error.WriteLine(result.Failure.UserMessage);
    return;
}
```

Use `StartTorrentsAsync` with the same selector to start it again. Supply `hash` from a torrent in your instance. These operations change the server's torrent state.

## Read torrent properties

```csharp
var result = await apiClient.GetTorrentPropertiesAsync(hash);
if (result.IsFailure)
{
    Console.Error.WriteLine(result.Failure.UserMessage);
    return;
}

var properties = result.Value;
Console.WriteLine(properties);
```

## Add a torrent

Set `torrentUrl` to a torrent URL or magnet link you intend to add. This example requests that it be added in a stopped state:

```csharp
var request = new AddTorrentParams
{
    Urls = [torrentUrl],
    Stopped = true
};

var result = await apiClient.AddTorrentAsync(request);
if (result.IsFailure)
{
    Console.Error.WriteLine(result.Failure.UserMessage);
    return;
}

if (result.IsPending)
{
    var pending = result.PendingValue;
    Console.WriteLine($"Pending: {pending.PendingCount}; added so far: {pending.SuccessCount}");
    return;
}

if (result.IsSuccess)
{
    var added = result.SuccessValue;
    Console.WriteLine($"Added: {added.SuccessCount}; failed: {added.FailureCount}");
}
```

The add result is an initial snapshot. Inspect its counts and returned IDs rather than assuming every submitted torrent was added. When additions are pending, observe the torrent list or sync state to determine whether the expected torrents appear. qBittorrent does not provide a follow-up add-operation status endpoint.

For polling and retry considerations, read [results and failures](results-and-failures.md).
