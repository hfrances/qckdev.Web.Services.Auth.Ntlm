using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Dto
{

    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.2.1
    /// </remarks>
    public enum AuthorizationErrorCode
    {
        /// <summary>
        /// The request is missing a required parameter, includes an invalid parameter value, includes a parameter more than once, or is otherwise malformed.
        /// </summary>
        [JsonPropertyName("invalid_request")]
        InvalidRequest,
        /// <summary>
        /// The client is not authorized to request an authorization code using this method.
        /// </summary>
        [JsonPropertyName("unauthorized_client")]
        UnauthorizedClient,
        /// <summary>
        /// The resource owner or authorization server denied the request.
        /// </summary>
        [JsonPropertyName("access_denied")]
        AccessDenied,
        /// <summary>
        /// The authorization server does not support obtaining an authorization code using this method.
        /// </summary>
        [JsonPropertyName("unsupported_response_type")]
        UnsupportedResponseType,
        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// </summary>
        [JsonPropertyName("invalid_scope")]
        InvalidScope,
        /// <summary>
        /// The authorization server encountered an unexpected condition that prevented it from fulfilling the request.
        /// (This error code is needed because a 500 Internal Server Error HTTP status code cannot be returned to the client via an HTTP redirect.)
        /// </summary>
        [JsonPropertyName("server_error")]
        ServerError,
        /// <summary>
        /// The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of the server.  
        /// (This error code is needed because a 503 Service Unavailable HTTP status code cannot be returned to the client via an HTTP redirect.)
        /// </summary>
        [JsonPropertyName("temporarily_unavailable")]
        TemporarilyUnavailable
    }

    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/rfc6749#section-7.1
    /// </remarks>
    public enum TokenType
    {
        /// <summary>
        /// Utilized by simply including the access token string in the request.
        /// </summary>
        [JsonPropertyName("bearer")]
        Bearer
    }

}
