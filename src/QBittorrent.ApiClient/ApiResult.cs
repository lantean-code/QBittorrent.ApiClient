using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Represents the outcome of a qBittorrent API operation that does not return a value.
    /// </summary>
    public sealed class ApiResult : ApiResultBase
    {
        /// <summary>
        /// Gets a value indicating whether the operation was accepted but has not completed yet.
        /// </summary>
        public bool IsPending => Status == ApiResultStatus.Pending;

        /// <summary>
        /// Initializes a new failed result.
        /// </summary>
        /// <param name="status">The result status.</param>
        /// <param name="failure">The failure details.</param>
        internal ApiResult(ApiResultStatus status, ApiFailure failure)
            : base(status, failure)
        {
        }

        /// <summary>
        /// Initializes a new non-failed result.
        /// </summary>
        /// <param name="status">The result status.</param>
        internal ApiResult(ApiResultStatus status)
            : base(status)
        {
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
        /// Creates a successful generic result.
        /// </summary>
        /// <param name="value">The returned value.</param>
        /// <returns>The successful result.</returns>
        public static ApiResult<TValue> CreateSuccess<TValue>(TValue value)
            where TValue : notnull
        {
            return new ApiResult<TValue>(ApiResultStatus.Success, value);
        }

        /// <summary>
        /// Creates a failed generic result.
        /// </summary>
        /// <typeparam name="TValue">The payload type.</typeparam>
        /// <param name="failure">The failure to return.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult<TValue> CreateFailure<TValue>(ApiFailure failure)
            where TValue : notnull
        {
            ArgumentNullException.ThrowIfNull(failure);

            return new ApiResult<TValue>(ApiResultStatus.Failure, default, failure);
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
    /// Represents the outcome of a qBittorrent API operation that returns a value.
    /// </summary>
    /// <typeparam name="TValue">The payload type.</typeparam>
    public sealed class ApiResult<TValue> : ApiResultBase
        where TValue : notnull
    {
        /// <summary>
        /// Gets the failure when the operation failed.
        /// </summary>
        public new ApiFailure? Failure => base.Failure;

        /// <summary>
        /// Gets the value when the operation completed successfully.
        /// </summary>
        public TValue? Value { get; }

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        public new bool IsSuccess => base.IsSuccess;

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Failure))]
        [MemberNotNullWhen(false, nameof(Value))]
        public new bool IsFailure => base.IsFailure;

        internal ApiResult(ApiResultStatus status, TValue? value, ApiFailure failure)
            : base(status, failure)
        {
            Value = value;
        }

        internal ApiResult(ApiResultStatus status, TValue? value)
            : base(status)
        {
            if (status != ApiResultStatus.Success)
            {
                throw new ArgumentException("Value results only support successful or failed states.", nameof(status));
            }

            Value = value;
        }

        /// <summary>
        /// Attempts to get the success value.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true" />, contains the value.</param>
        /// <returns><see langword="true" /> when the operation completed successfully; otherwise, <see langword="false" />.</returns>
        [MemberNotNullWhen(true, nameof(Value))]
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
    }

    /// <summary>
    /// Represents the outcome of a qBittorrent API operation whose success and pending states use different payload types.
    /// </summary>
    /// <typeparam name="TSuccess">The success payload type.</typeparam>
    /// <typeparam name="TPending">The pending payload type.</typeparam>
    public sealed class ApiResult<TSuccess, TPending> : ApiResultBase
        where TSuccess : notnull
        where TPending : notnull
    {
        /// <summary>
        /// Gets the failure when the operation failed.
        /// </summary>
        public new ApiFailure? Failure => base.Failure;

        /// <summary>
        /// Gets the success value when the operation completed successfully.
        /// </summary>
        public TSuccess? SuccessValue { get; }

        /// <summary>
        /// Gets the pending value when the operation is still pending.
        /// </summary>
        public TPending? PendingValue { get; }

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        [MemberNotNullWhen(true, nameof(SuccessValue))]
        public new bool IsSuccess => base.IsSuccess;

        /// <summary>
        /// Gets a value indicating whether the operation was accepted but has not completed yet.
        /// </summary>
        [MemberNotNullWhen(true, nameof(PendingValue))]
        public bool IsPending => Status == ApiResultStatus.Pending;

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Failure))]
        public new bool IsFailure => base.IsFailure;

        internal ApiResult(ApiResultStatus status, TSuccess? successValue, TPending? pendingValue, ApiFailure failure)
            : base(status, failure)
        {
            SuccessValue = successValue;
            PendingValue = pendingValue;
        }

        internal ApiResult(ApiResultStatus status, TSuccess? successValue, TPending? pendingValue)
            : base(status)
        {
            if (status == ApiResultStatus.Success)
            {
            }
            else if (status == ApiResultStatus.Pending)
            {
            }
            else
            {
                throw new ArgumentException("Dual-payload results only support successful, pending, or failed states.", nameof(status));
            }

            SuccessValue = successValue;
            PendingValue = pendingValue;
        }

        /// <summary>
        /// Attempts to get the success value.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true" />, contains the success value.</param>
        /// <returns><see langword="true" /> when the operation completed successfully; otherwise, <see langword="false" />.</returns>
        [MemberNotNullWhen(true, nameof(SuccessValue))]
        public bool TryGetSuccessValue([NotNullWhen(true)] out TSuccess? value)
        {
            if (IsSuccess)
            {
                value = SuccessValue;
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
        [MemberNotNullWhen(true, nameof(PendingValue))]
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
