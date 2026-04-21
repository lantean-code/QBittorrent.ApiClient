using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    /// <summary>
    /// Represents the common outcome state of a qBittorrent API operation.
    /// </summary>
    public abstract class ApiResultBase
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
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Failure))]
        public bool IsFailure => Status == ApiResultStatus.Failure;

        /// <summary>
        /// Initializes a new failed result.
        /// </summary>
        /// <param name="status">The result status.</param>
        /// <param name="failure">The failure details.</param>
        protected ApiResultBase(ApiResultStatus status, ApiFailure failure)
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
        protected ApiResultBase(ApiResultStatus status)
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
    }
}
