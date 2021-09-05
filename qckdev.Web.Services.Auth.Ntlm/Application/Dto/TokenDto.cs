using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Dto
{
    public class TokenDto
    {

        /// <summary>
        /// The access token issued by the authorization server.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The type of the token issued. Value is case insensitive.
        /// </summary>
        [JsonPropertyName("token_type")]
        public TokenType? TokenType { get; set; }

        /// <summary>
        /// The lifetime in seconds of the access token.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public double? ExpiresIn { get; set; }

        /// <summary>
        /// This field is only present if the access_type parameter was set to 'offline' in the authentication request.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

    }
}
