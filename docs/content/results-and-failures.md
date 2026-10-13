# Results and failures

API calls return structured results. Handle failure first and return, then continue with the successful value. This keeps the main operation outside nested result checks.

## Read a successful value

```csharp
var result = await apiClient.GetApplicationVersionAsync();
if (result.IsFailure)
{
    Console.Error.WriteLine(result.Failure.UserMessage);
    return;
}

var version = result.Value;
Console.WriteLine(version);
```

`ApiResult<TValue>` has only success and failure states. After the failure guard returns, `Value` is available. The result's nullable annotations let the compiler follow that check without a null-forgiving operator. `TryGetValue` remains available when an out-value check fits your code better.

For calls without a value, handle `IsFailure` before continuing. For operations that can be pending, handle that state separately. `IsSuccess` means the API operation completed successfully; starting a torrent, for example, does not mean its download has finished.

## Handle pending operations

The current client also exposes `ApiResultStatus.Pending` for operations accepted by qBittorrent that have not completed. A pending result has `IsSuccess == false` and `IsFailure == false`. Do not assume every unsuccessful result has a `Failure`.

Some operations use `ApiResult<TSuccess, TPending>`. Handle `IsFailure` first, then check `IsPending` to read `PendingValue`. Read `SuccessValue` under an `IsSuccess` check. These checks establish which payload is available to the compiler. See [adding a torrent](common-operations.md#add-a-torrent) for an example.

Pending is not a completed, usable outcome. Your application must observe the relevant server state or poll the operation's status endpoint where one exists. Do not submit the same operation again merely because the first result is pending.

## Inspect a failure

```csharp
var result = await apiClient.GetApplicationVersionAsync();
if (result.IsFailure)
{
    var failure = result.Failure;
    Console.Error.WriteLine($"{failure.Operation}: {failure.UserMessage}");
    Console.Error.WriteLine($"Failure kind: {failure.Kind}");
    return;
}
```

`ApiFailure` contains:

| Detail | Use |
| --- | --- |
| `Kind` | Branch on a broad failure category such as authentication required, validation failed, timeout, or unexpected response. |
| `Operation` | Identify the client operation that failed. |
| `UserMessage` | Show an explanation when your application has no more specific message. |
| `Reason` / `TryGetReason<TReason>` | Inspect a known operation-specific reason. |
| `IsTransient` | Assess whether a later attempt might succeed. |
| `StatusCode`, `Detail`, `ResponseBody` | Diagnose the failure when these details are available. |

Choose a retry policy based on the operation as well as `IsTransient`. A timeout can leave the server's outcome unknown; retrying a mutation can repeat work. Take care when recording raw response bodies or diagnostic details that might contain application data.

## Exceptions and cancellation

Results represent normal API outcomes. Invalid arguments and incorrect usage can still throw exceptions. In particular, calling a compatibility-gated operation before successful initialization throws `InvalidOperationException`.

Pass a `CancellationToken` to calls when your application needs to stop work. Caller-requested cancellation propagates as an exception; it is separate from a timeout failure returned by the client.
