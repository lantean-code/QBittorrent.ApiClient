using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Represents the outcome of a qBittorrent API operation that does not return a value.
    /// </summary>
    public class ApiResult
    {
        private ApiResult(ApiFailure? failure)
        {
            Failure = failure;
        }

        /// <summary>
        /// Gets the failure when the operation did not succeed.
        /// </summary>
        public ApiFailure? Failure { get; }

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Failure))]
        public bool IsSuccess => Failure is null;

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>The successful result.</returns>
        public static ApiResult Success()
        {
            return new ApiResult(null);
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="value">The returned value.</param>
        /// <returns>The successful result.</returns>
        public static ApiResult<T> Success<T>(T value)
        {
            return ApiResult<T>.Success(value);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="failure">The failure to return.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult FailureResult(ApiFailure failure)
        {
            ArgumentNullException.ThrowIfNull(failure);

            return new ApiResult(failure);
        }

        /// <summary>
        /// Attempts to get the failure.
        /// </summary>
        /// <param name="failure">When this method returns <see langword="true" />, contains the failure.</param>
        /// <returns><see langword="true" /> when the operation failed; otherwise, <see langword="false" />.</returns>
        [MemberNotNullWhen(true, nameof(Failure))]
        public bool TryGetFailure([NotNullWhen(true)] out ApiFailure? failure)
        {
            failure = Failure;
            return !IsSuccess;
        }
    }

    /// <summary>
    /// Represents the outcome of a qBittorrent API operation that returns a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public sealed class ApiResult<T>
    {
        internal ApiResult(T? value, ApiFailure? failure)
        {
            Value = value;
            Failure = failure;
        }

        /// <summary>
        /// Gets the value when the operation succeeded.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Gets the failure when the operation did not succeed.
        /// </summary>
        public ApiFailure? Failure { get; }

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        [MemberNotNullWhen(false, nameof(Failure))]
        public bool IsSuccess => Failure is null;

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="value">The returned value.</param>
        /// <returns>The successful result.</returns>
        public static ApiResult<T> Success(T value)
        {
            return new ApiResult<T>(value, null);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="failure">The failure to return.</param>
        /// <returns>The failed result.</returns>
        public static ApiResult<T> FailureResult(ApiFailure failure)
        {
            ArgumentNullException.ThrowIfNull(failure);

            return new ApiResult<T>(default, failure);
        }

        /// <summary>
        /// Attempts to get the value.
        /// </summary>
        /// <param name="value">When this method returns <see langword="true" />, contains the value.</param>
        /// <returns><see langword="true" /> when the operation succeeded; otherwise, <see langword="false" />.</returns>
        [MemberNotNullWhen(true, nameof(Value))]
        [MemberNotNullWhen(false, nameof(Failure))]
        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            value = Value is null ? default : Value;
            return IsSuccess;
        }

        /// <summary>
        /// Attempts to get the failure.
        /// </summary>
        /// <param name="failure">When this method returns <see langword="true" />, contains the failure.</param>
        /// <returns><see langword="true" /> when the operation failed; otherwise, <see langword="false" />.</returns>
        [MemberNotNullWhen(true, nameof(Failure))]
        public bool TryGetFailure([NotNullWhen(true)] out ApiFailure? failure)
        {
            failure = Failure;
            return !IsSuccess;
        }
    }
}
