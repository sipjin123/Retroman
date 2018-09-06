using System.Collections.Generic;
using UnityEngine;

namespace Framework.Services.Implementation
{
    /// <summary>
    /// A generic implementation of IResponse.
    /// </summary>
    public class GenericResponse : IResponse
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericResponse"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        public GenericResponse(string id, string requestId)
        {
            Id = id;
            RequestId = requestId;
            ResponseString = string.Empty;
            ErrorString = string.Empty;
            ResponseData = new Dictionary<string, object>();
            ErrorData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericResponse"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="responseString">The response string.</param>
        /// <param name="errorString">The error string.</param>
        public GenericResponse(string id, string requestId, string responseString, string errorString = "")
            : this(id, requestId)
        {
            ResponseString = responseString;
            ErrorString = errorString;
            ResponseData = new Dictionary<string, object> { { "response", ResponseString } };
            ErrorData = new Dictionary<string, object> { { "error", ErrorString } };
        }

        #endregion Ctor

        #region IResponse Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; protected set; }

        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public string RequestId { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasErrors { get { return !string.IsNullOrEmpty(ErrorString); } }

        /// <summary>
        /// Gets the response string.
        /// </summary>
        /// <value>
        /// The response string.
        /// </value>
        public string ResponseString { get; protected set; }

        /// <summary>
        /// Gets the error string.
        /// </summary>
        /// <value>
        /// The error string.
        /// </value>
        public string ErrorString { get; protected set; }

        /// <summary>
        /// Gets the response data.
        /// </summary>
        /// <value>
        /// The response data.
        /// </value>
        public IDictionary<string, object> ResponseData { get; protected set; }

        /// <summary>
        /// Gets the error data.
        /// </summary>
        /// <value>
        /// The error data.
        /// </value>
        public IDictionary<string, object> ErrorData { get; protected set; }

        #endregion IResponse Properties

        #region IResponse Methods

        /// <summary>
        /// Prints the errors.
        /// </summary>
        /// <returns></returns>
        public virtual string PrintErrors()
        {
            Debug.LogError(ErrorString);

            return ErrorString;
        }

        #endregion IResponse Methods
    }
}