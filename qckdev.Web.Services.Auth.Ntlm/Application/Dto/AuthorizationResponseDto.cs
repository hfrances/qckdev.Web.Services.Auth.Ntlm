﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Dto
{

    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.1
    /// </remarks>
    public sealed class AuthorizationResponseDto : TokenDto
    {
        /// <summary>
        /// The authorization code generated by the authorization server.
        /// The client MUST NOT use the authorization code more than once. 
        /// If an authorization code is used more than once, the authorization server MUST deny the request
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The scope of the access token. Can be indentical to the scope requested by the client.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// If the "state" parameter was present in the client authorization request.The exact value received from the client.
        /// </summary>
        public string State { get; set; }


    }
}
