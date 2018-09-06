using System.Collections.Generic;

namespace Framework.Services
{
    /// <summary>
    /// An interface where all responses should implement from.
    /// </summary>
    public interface IResponse
    {
        #region Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        string RequestId { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }

        /// <summary>
        /// Gets the response string.
        /// </summary>
        /// <value>
        /// The response string.
        /// </value>
        string ResponseString { get; }

        /// <summary>
        /// Gets the error string.
        /// </summary>
        /// <value>
        /// The error string.
        /// </value>
        string ErrorString { get; }

        /// <summary>
        /// Gets the response data.
        /// </summary>
        /// <value>
        /// The response data.
        /// </value>
        IDictionary<string, object> ResponseData { get; }

        /// <summary>
        /// Gets the error data.
        /// </summary>
        /// <value>
        /// The error data.
        /// </value>
        IDictionary<string, object> ErrorData { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Prints the errors.
        /// </summary>
        /// <returns></returns>
        string PrintErrors();

        #endregion
    }
}