using MediatR;
using Microsoft.AspNetCore.Mvc;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Models;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Queries
{

    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/rfc6749#section-3.1.1
    /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.1
    /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.2.1
    /// https://developers.google.com/identity/protocols/oauth2/openid-connect#authenticatingtheuser
    /// </remarks>
    public sealed class AuthorizationRequestQuery: IRequest<AuthorizationResponseDto>
    {

        /// <summary>
        /// The client ID string that you obtain from the API Console Credentials page, as described in Obtain OAuth 2.0 credentials.
        /// </summary>
        [ModelBinder(Name = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// If the value is 'code', launches a Basic authorization code flow, requiring a POST to the token endpoint to obtain the tokens. 
        /// If the value is 'token', launches an Implicit flow, requiring the use of JavaScript at the redirect URI to retrieve tokens from the URI #fragment identifier.
        /// </summary>
        [ModelBinder(Name = "response_type")]
        public ResponseType ResponseType { get; set; }

        /// <summary>
        /// Determines where the response is sent. 
        /// The value of this parameter must exactly match one of the authorized redirect values that you set in the API Console Credentials page 
        /// (including the HTTP or HTTPS scheme, case, and trailing '/', if any).
        /// </summary>
        [ModelBinder(Name = "redirect_uri")]
        public string RedirectUri { get; set; }

        /// <summary>
        /// The scope parameter must begin with the openid value and then include the profile value, the email value, or both.
        /// If the profile scope value is present, the ID token might(but is not guaranteed to) include the user's default profile claims.
        /// If the email scope value is present, the ID token includes email and email_verified claims.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// An opaque string that is round-tripped in the protocol; that is to say, it is returned as a URI parameter in the Basic flow, 
        /// and in the URI #fragment identifier in the Implicit flow.
        /// The state can be useful for correlating requests and responses.Because your redirect_uri can be guessed, using a state value can increase your assurance that an incoming 
        /// connection is the result of an authentication request initiated by your app.If you generate a random string or encode the hash of some client state (e.g., a cookie) 
        /// in this state variable, you can validate the response to additionally ensure that the request and response originated in the same browser.
        /// This provides protection against attacks such as cross-site request forgery.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The allowed values are offline and online. The effect is documented in Offline Access; 
        /// if an access token is being requested, the client does not receive a refresh token unless a value of offline is specified.
        /// </summary>
        [ModelBinder(Name = "access_type")]
        public AccessType AccessType { get; set; }

    }
}
