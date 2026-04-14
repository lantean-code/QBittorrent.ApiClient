using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Represents the outcome of a qBittorrent API operation that does not return a value.
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Gets the normalized result status.
        /// </summary>
        public ApiResultStatus Status { get; }

        /// <summary>
        /// Gets the failure when the operation failed.
        /// </summary>
        public ApiFailure? Failure { get; }

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool IsSuccess => Status == ApiResultStatus.Success;

        /// <summary>
        /// Gets a value indicating whether the operation was accepted but has not completed yet.
        /// </summary>
        public bool IsPending => Status == ApiResultStatus.Pending;

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Failure))]
        public bool IsFailure => Status == ApiResultStatus.Failure;

        /// <summary>
        /// Initializes a new failed result.
        /// </summary>
        /// <param name="status">The result status.</param>
        /// <param name="failure">The failure details.</param>
        protected ApiResult(ApiResultStatus status, ApiFailure failure)
        {
            ArgumentNullException.ThrowIfNull(failure);

            if (status != ApiResultStatus.Failure)
            {
                throw new ArgumentException("Failure details can only be provided for failed results.", nameof(status));
            }

            Status = status;
            Failure = failure;
        }

        /// <summary>
        /// Initializes a new non-failed result.
        /// </summary>
        /// <param name="status">The result status.</param>
        protected ApiResult(ApiResultStatus status)
        {
            if (status == ApiResultStatus.Failure)
            {
                throw new ArgumentException("Failed results require failure details.", nameof(status));
            }

            Status = status;
        }

        /// <summary>
        /// Attempts to get the failure.
        /// </summary>
        /// <param name="failure">When this method returns <see langword="true" />, contains the failure.</param>
        /// <returns><see langword="true" /> when the operation failed; otherwise, <see langword="false" />.</returns>
        [MemberNotNullWhen(true, nameof(Failure))]
        public bool TryGetFailure([NotNullWhen(true)] out ApiFailure? failure)
        {
            if (IsFailure)
            {
                failure = Failure;
                return true;
            }

            failure = default;
            return false;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>The successful result.</returns>
        public static ApiResult CreateSuccess()
        {
            return new ApiResult(ApiResultStatus.Success);
        }

        /// <summary>
        /// Creates a pending result.
        /// </summary>
        /// <returns>The pending result.</returns>
        public static ApiResult CreatePending()
        {
            return new ApiResult(ApiResultStatus.Pending);
        }

        /// <summary>
        /// Creates a successful generic result whose success and pending states share the same payload type.
        /// </summary>
        /// <param name="value">The returned value.</param>
        /// <returns>The successful result.</returns>
        public static ApiResult<TValue> CreateSuccess<TValue>(TValue value)
            where TValue : notnull
        {
            return new ApiResult<TValue>(ApiResultStatus.Success, value, default);
        }

        /// <summary>
        /// Creates a pending generic result whose success and pending states share the same payload type.
        /// </summary>
        /// <typeparam name="TValue">The payload type.</typeparam>
        /// <param name="value">The pending value.</param>
        /// <returns>The pending result.</returns>
        public static ApiResult<TValue> CreatePending<TValue>(TValue value)
            where TValue : notnull
        {
            return new ApiResult<TValue>(ApiResultStatus.Pending, default, value);
        }

        /// <summary>
        /// Creates a failed generic result whose success and pending states share the same payload type.
        /// </summary>
        /// <typeparam name="TValue">The payload type.</typeparam>
        /// <param name="failure">The failure to return.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult<TValue> CreateFailure<TValue>(ApiFailure failure)
            where TValue : notnull
        {
            ArgumentNullException.ThrowIfNull(failure);

            return new ApiResult<TValue>(ApiResultStatus.Failure, default, default, failure);
        }

        /// <summary>
        /// Creates a successful dual-payload result.
        /// </summary>
        /// <typeparam name="TSuccess">The success payload type.</typeparam>
        /// <typeparam name="TPending">The pending payload type.</typeparam>
        /// <param name="value">The returned success value.</param>
        /// <returns>The successful result.</returns>
        public static ApiResult<TSuccess, TPending> CreateSuccess<TSuccess, TPending>(TSuccess value)
            where TSuccess : notnull
            where TPending : notnull
        {
            return new ApiResult<TSuccess, TPending>(ApiResultStatus.Success, value, default);
        }

        /// <summary>
        /// Creates a pending dual-payload result.
        /// </summary>
        /// <typeparam name="TSuccess">The success payload type.</typeparam>
        /// <typeparam name="TPending">The pending payload type.</typeparam>
        /// <param name="value">The pending value.</param>
        /// <returns>The pending result.</returns>
        public static ApiResult<TSuccess, TPending> CreatePending<TSuccess, TPending>(TPending value)
            where TSuccess : notnull
            where TPending : notnull
        {
            return new ApiResult<TSuccess, TPending>(ApiResultStatus.Pending, default, value);
        }

        /// <summary>
        /// Creates a failed dual-payload result.
        /// </summary>
        /// <typeparam name="TSuccess">The success payload type.</typeparam>
        /// <typeparam name="TPending">The pending payload type.</typeparam>
        /// <param name="failure">The failure to return.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult<TSuccess, TPending> CreateFailure<TSuccess, TPending>(ApiFailure failure)
            where TSuccess : notnull
            where TPending : notnull
        {
            ArgumentNullException.ThrowIfNull(failure);

            return new ApiResult<TSuccess, TPending>(ApiResultStatus.Failure, default, default, failure);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="failure">The failure to return.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult CreateFailure(ApiFailure failure)
        {
            ArgumentNullException.ThrowIfNull(failure);

            return new ApiResult(ApiResultStatus.Failure, failure);
        }
    }

    /// <summary>
    /// Represents the outcome of a qBittorrent API operation whose success and pending states share the same payload type.
    /// </summary>
    /// <typeparam name="TValue">The payload type.</typeparam>
    public sealed class ApiResult<TValue> : ApiResult
        where TValue : notnull
    {
        /// <summary>
        /// Gets the value when the operation completed successfully.
        /// </summary>
        public TValue? Value { get; }

        /// <summary>
        /// Gets the value when the operation is still pending.
        /// </summary>
        public TValue? PendingValue { get; }

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        public new bool IsSuccess => base.IsSuccess;

        /// <summary>
        /// Gets a value indicating whether the operation was accepted but has not completed yet.
        /// </summary>
        [MemberNotNullWhen(true, nameof(PendingValue))]
        public new bool IsPending => base.IsPending;

        internal ApiResult(ApiResultStatus status, TValue? value, TValue? pendingValue, ApiFailure failure)
            : base(status, failure)
        {
            Value = value;
            PendingValue = pendingValue;
        }

        internal ApiResult(ApiResultStatus status, TValue? value, TValue? pendingValue)
            : base(status)
        {
            Value = value;
            PendingValue = pendingValue;
        }

        /// <summary>
        /// Attempts to get the success value.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true" />, contains the value.</param>
        /// <returns><see langword="true" /> when the operation completed successfully; otherwise, <see langword="false" />.</returns>
        public bool TryGetValue([NotNullWhen(true)] out TValue? value)
        {
            if (IsSuccess)
            {
                value = Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the pending value.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true" />, contains the pending value.</param>
        /// <returns><see langword="true" /> when the operation is pending and carries a value; otherwise, <see langword="false" />.</returns>
        public bool TryGetPendingValue([NotNullWhen(true)] out TValue? value)
        {
            if (IsPending)
            {
                value = PendingValue;
                return true;
            }

            value = default;
            return false;
        }

    }

    /// <summary>
    /// Represents the outcome of a qBittorrent API operation whose success and pending states use different payload types.
    /// </summary>
    /// <typeparam name="TSuccess">The success payload type.</typeparam>
    /// <typeparam name="TPending">The pending payload type.</typeparam>
    public sealed class ApiResult<TSuccess, TPending> : ApiResult
        where TSuccess : notnull
        where TPending : notnull
    {
        /// <summary>
        /// Gets the value when the operation completed successfully.
        /// </summary>
        public TSuccess? Value { get; }

        /// <summary>
        /// Gets the value when the operation is still pending.
        /// </summary>
        public TPending? PendingValue { get; }

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        public new bool IsSuccess => base.IsSuccess;

        /// <summary>
        /// Gets a value indicating whether the operation was accepted but has not completed yet.
        /// </summary>
        [MemberNotNullWhen(true, nameof(PendingValue))]
        public new bool IsPending => base.IsPending;

        internal ApiResult(ApiResultStatus status, TSuccess? successValue, TPending? pendingValue, ApiFailure failure)
            : base(status, failure)
        {
            Value = successValue;
            PendingValue = pendingValue;
        }

        internal ApiResult(ApiResultStatus status, TSuccess? successValue, TPending? pendingValue)
            : base(status)
        {
            Value = successValue;
            PendingValue = pendingValue;
        }

        /// <summary>
        /// Attempts to get the success value.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true" />, contains the success value.</param>
        /// <returns><see langword="true" /> when the operation completed successfully; otherwise, <see langword="false" />.</returns>
        public bool TryGetValue([NotNullWhen(true)] out TSuccess? value)
        {
            if (IsSuccess)
            {
                value = Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the pending value.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true" />, contains the pending value.</param>
        /// <returns><see langword="true" /> when the operation is pending; otherwise, <see langword="false" />.</returns>
        public bool TryGetPendingValue([NotNullWhen(true)] out TPending? value)
        {
            if (IsPending)
            {
                value = PendingValue;
                return true;
            }

            value = default;
            return false;
        }

    }
}
