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
    public sealed class OAuth2ErrorDto
    {

        /// <summary>
        /// A single ASCII error code.
        /// </summary>
        [JsonPropertyName("error")]
        public AuthorizationErrorCode Code { get; set; }

        /// <summary>
        /// Human-readable ASCII text providing additional information, used to assist the client developer in understanding the error that occurred.
        /// </summary>
        [JsonPropertyName("error_description")]
        public string Description { get; set; }

        /// <summary>
        /// A URI identifying a human-readable web page with information about the error, used to provide the client developer with additional information about the error.
        /// </summary>
        [JsonPropertyName("error_uri")]
        public string Uri { get; set; }

        /// <summary>
        /// Required if a "state" parameter was present in the client authorization request. The exact value received from the client.
        /// </summary>
        public string State { get; set; }

    }
}
