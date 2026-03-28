using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Describes a normalized qBittorrent API failure.
    /// </summary>
    public sealed record ApiFailure
    {
        /// <summary>
        /// Gets the broad class of failure.
        /// </summary>
        public required ApiFailureKind Kind { get; init; }

        /// <summary>
        /// Gets the qBittorrent API operation that produced the failure.
        /// </summary>
        public required string Operation { get; init; }

        /// <summary>
        /// Gets the HTTP status code, when one was received.
        /// </summary>
        public HttpStatusCode? StatusCode { get; init; }

        /// <summary>
        /// Gets a localized-ready message that callers can display directly when they do not have a more specific UX.
        /// </summary>
        public required string UserMessage { get; init; }

        /// <summary>
        /// Gets additional failure detail intended for diagnostics.
        /// </summary>
        public string? Detail { get; init; }

        /// <summary>
        /// Gets the operation-specific failure reason, when one is known.
        /// </summary>
        public object? Reason { get; init; }

        /// <summary>
        /// Gets a value indicating whether the failure is likely to be transient.
        /// </summary>
        public bool IsTransient { get; init; }

        /// <summary>
        /// Gets the raw response body, when one was returned.
        /// </summary>
        public string? ResponseBody { get; init; }

        /// <summary>
        /// Attempts to get the operation-specific reason as the requested enum type.
        /// </summary>
        /// <typeparam name="TReason">The scoped reason enum type.</typeparam>
        /// <param name="reason">When this method returns <see langword="true" />, contains the typed reason.</param>
        /// <returns><see langword="true" /> when the reason could be read as <typeparamref name="TReason" />; otherwise, <see langword="false" />.</returns>
        public bool TryGetReason<TReason>([NotNullWhen(true)] out TReason? reason)
            where TReason : struct, Enum
        {
            if (Reason is TReason typedReason)
            {
                reason = typedReason;
                return true;
            }

            reason = null;
            return false;
        }
    }
}
