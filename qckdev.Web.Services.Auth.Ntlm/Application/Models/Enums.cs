using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Models
{
    public enum ResponseType
    {
        /// <summary>
        /// Unset value.
        /// </summary>
        Uknown,
        /// <summary>
        /// for requesting an authorization code.
        /// </summary>
        Code,
        /// <summary>
        /// for requesting an access token (implicit grant).
        /// </summary>
        Token
    }

    public enum AccessType
    {
        Online,
        Offline
    }

    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.3
    /// </remarks>
    public enum GrantType
    {
        /// <summary>
        /// Unset value.
        /// </summary>
        Unknown,
        /// <summary>
        /// Makes a request for getting an access token using the authorization code.
        /// </summary>
        Authorization_Code,
        /// <summary>
        /// Creates a new acces token from the refresh token.
        /// </summary>
        Refresh_Token,
        /// <summary>
        /// Makes a request for getting an access token using some authorization header method. Not implemented.
        /// </summary>
        Client_Credentials,
        /// <summary>
        /// Makes a request for getting an access token passing login and password. Not implemented.
        /// </summary>
        Password,
    }

    public enum TokenState
    {
        Active = 0,
        Consumed = 50,
        Banned = 90
    }

}
