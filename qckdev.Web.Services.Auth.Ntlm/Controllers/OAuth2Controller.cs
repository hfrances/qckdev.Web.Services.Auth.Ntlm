using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using qckdev.Web.Services.Auth.Ntlm.Application.Commands;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Exceptions;
using qckdev.Web.Services.Auth.Ntlm.Application.Helpers;
using qckdev.Web.Services.Auth.Ntlm.Application.Queries;
using System;
using System.Threading.Tasks;
using System.Web;

namespace qckdev.Web.Services.Auth.Ntlm.Controllers
{
    [Route("oauth2")]
    [ApiController, Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
    public sealed class OAuth2Controller : ControllerBase
    {

        IMediator Mediator { get; }

        public OAuth2Controller(IMediator mediator)
        {
            this.Mediator = mediator;
        }

        /// <summary>
        /// Requests authorization code or token.
        /// </summary>
        /// <remarks>
        /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.1
        /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.2
        /// </remarks>
        [HttpGet("authorize")]
        public async Task<IActionResult> GetAuthorize([FromQuery] AuthorizationRequestQuery request)
            => await Authorize(request);

        /// <summary>
        /// Requests authorization code or token.
        /// </summary>
        /// <remarks>
        /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.1
        /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.2
        /// </remarks>
        [HttpPost("authorize"), Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> PostAuthorize([FromForm] AuthorizationRequestQuery request)
            => await Authorize(request);

        /// <summary>
        /// Request token from authorization code or refresh token.
        /// </summary>
        /// <remarks>
        /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.3
        /// https://datatracker.ietf.org/doc/html/rfc6749#section-4.4.1
        /// https://developers.google.com/identity/protocols/oauth2/openid-connect#exchangecode
        /// </remarks>
        [HttpPost("token"), AllowAnonymous, Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<TokenDto>> CreateToken([FromForm] CreateTokenCommand request)
        {

            try
            {
                var response = await Mediator.Send(request);

                return Ok(response);
            }
            catch (OAuth2Exception ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpGet("user"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserDto>> GetUser([FromQuery] GetUserQuery request)
        {
            return Ok(await Mediator.Send(request));
        }

        private async Task<IActionResult> Authorize(AuthorizationRequestQuery request)
        {
            try
            {
                var response = await Mediator.Send(request);
                var uriBuilder = new UriBuilder(request.RedirectUri);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                if (!string.IsNullOrEmpty(response.Code))
                {
                    query["code"] = response.Code;
                }
                if (!string.IsNullOrEmpty(response.AccessToken))
                {
                    query["access_token"] = response.AccessToken;
                }
                if (response.TokenType != null)
                {
                    query["token_type"] = response.TokenType.Value.ToStringJson();
                }
                if (response.ExpiresIn != null)
                {
                    query["expires_in"] = response.ExpiresIn.ToString();
                }
                if (!string.IsNullOrEmpty(response.Scope) && !request.Scope.Equals(request.Scope))
                {
                    query["scope"] = response.Scope;
                }
                if (!string.IsNullOrEmpty(request.State))
                {
                    query["state"] = response.State;
                }
                uriBuilder.Query = query.ToString();
                return Redirect(uriBuilder.Uri.ToString());
            }
            catch (OAuth2Exception ex)
            {
                var uriBuilder = new UriBuilder(request.RedirectUri);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                query["error"] = ex.Error.Code.ToStringJson();
                if (!string.IsNullOrEmpty(ex.Error.Description))
                {
                    query["error_description"] = ex.Error.Description;
                }
                if (!string.IsNullOrEmpty(ex.Error.Uri))
                {
                    query["error_uri"] = ex.Error.Uri;
                }
                if (!string.IsNullOrEmpty(ex.Error.State))
                {
                    query["state"] = ex.Error.State;
                }
                uriBuilder.Query = query.ToString();
                return Redirect(uriBuilder.Uri.ToString());
            }
        }



    }
}
