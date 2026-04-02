namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Represents the broad class of failure returned by the qBittorrent API client.
    /// </summary>
    public enum ApiFailureKind
    {
        /// <summary>
        /// The current session is not authenticated.
        /// </summary>
        AuthenticationRequired,

        /// <summary>
        /// Authentication was attempted but rejected by qBittorrent.
        /// </summary>
        AuthenticationRejected,

        /// <summary>
        /// The current session is authenticated, but the requested action was denied.
        /// </summary>
        AccessDenied,

        /// <summary>
        /// The requested resource does not exist.
        /// </summary>
        NotFound,

        /// <summary>
        /// The supplied input was rejected as invalid.
        /// </summary>
        ValidationFailed,

        /// <summary>
        /// The request conflicts with the current server state.
        /// </summary>
        Conflict,

        /// <summary>
        /// The supplied data format is unsupported.
        /// </summary>
        UnsupportedData,

        /// <summary>
        /// The API client is not configured correctly.
        /// </summary>
        InvalidConfiguration,

        /// <summary>
        /// The qBittorrent host could not be reached.
        /// </summary>
        NoResponse,

        /// <summary>
        /// The request timed out before a response was received.
        /// </summary>
        Timeout,

        /// <summary>
        /// The operation was accepted by qBittorrent but has not completed yet.
        /// </summary>
        OperationPending,

        /// <summary>
        /// The server returned an error response.
        /// </summary>
        ServerError,

        /// <summary>
        /// The response was syntactically valid but could not be interpreted as the expected payload.
        /// </summary>
        UnexpectedResponse
    }
}
