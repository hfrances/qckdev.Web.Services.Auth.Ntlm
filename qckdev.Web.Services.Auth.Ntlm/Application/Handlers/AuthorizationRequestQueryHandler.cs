using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Exceptions;
using qckdev.Web.Services.Auth.Ntlm.Application.Helpers;
using qckdev.Web.Services.Auth.Ntlm.Application.Queries;
using qckdev.Web.Services.Auth.Ntlm.Application.Services;
using qckdev.Web.Services.Auth.Ntlm.Infrastructure.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Handlers
{
    public sealed class AuthorizationRequestQueryHandler : IRequestHandler<AuthorizationRequestQuery, AuthorizationResponseDto>
    {

        IHttpContextAccessor HttpContextAccessor { get; }
        IOptionsMonitor<JwtBearerOptions> JwtOptionsMonitor { get; }
        IOptionsMonitor<JwtBearerMoreOptions> JwtMoreOptionsMonitor { get; }
        ITokenService TokenService { get; }

        public AuthorizationRequestQueryHandler(
            IHttpContextAccessor httpContextAccessor, IOptionsMonitor<JwtBearerOptions> jwtOptionsMonitor, IOptionsMonitor<JwtBearerMoreOptions> jwtMoreOptionsMonitor,
            ITokenService tokenService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.JwtOptionsMonitor = jwtOptionsMonitor;
            this.JwtMoreOptionsMonitor = jwtMoreOptionsMonitor;
            this.TokenService = tokenService;
        }

        public async Task<AuthorizationResponseDto> Handle(AuthorizationRequestQuery request, CancellationToken cancellationToken)
        {

            var user = HttpContextAccessor.HttpContext.User;

            if (string.IsNullOrEmpty(user.Identity.Name))
            {
                throw new OAuth2Exception(new OAuth2ErrorDto
                {
                    Code = AuthorizationErrorCode.UnauthorizedClient,
                    Description = "No user has been authenticated.",
                    State = request.State,
                });
            }
            else
            {
                switch (request.ResponseType)
                {
                    case Models.ResponseType.Code:
                        var jwtCodeOptions = JwtOptionsMonitor.Get(Constants.AUTHENTICATIONSCHEME_CODE);
                        var jwtCodeMoreOptions = JwtMoreOptionsMonitor.Get(Constants.AUTHENTICATIONSCHEME_CODE);
                        var claims = new List<Claim>(user.Claims)
                        {
                            new Claim(Helper.CLAIM_ACCESSTYPE, request.AccessType.ToString())
                        };
                        var keyCode = jwtCodeOptions.TokenValidationParameters.IssuerSigningKey;
                        var tmpCode = JwtGenerator.CreateToken(keyCode, user.Identity.Name, claims: claims, lifespan: jwtCodeMoreOptions.TokenLifeTimespan);
                        var code = $"4/{tmpCode.AccessToken}";

                        await Helper.UpdateCodeTokenAsync(this.TokenService, user.Identity.Name, code, tmpCode.Expired);
                        return await Task.FromResult(new AuthorizationResponseDto
                        {
                            Code = code,
                            State = request.State
                        });

                    case Models.ResponseType.Token:
                        var jwtTokenOptions = JwtOptionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
                        var jwtTokenMoreOptions = JwtMoreOptionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
                        AuthorizationResponseDto token;

                        token = Helper.CreateToken<AuthorizationResponseDto>(jwtTokenOptions, jwtTokenMoreOptions, user.Identity.Name, user.Claims, request.AccessType);
                        await Helper.UpdateRefreshTokenAsync(this.TokenService, user.Identity.Name, token.RefreshToken);
                        return token;

                    default:
                        throw new OAuth2Exception(new OAuth2ErrorDto
                        {
                            Code = AuthorizationErrorCode.UnsupportedResponseType,
                            Description = $"Invalid value for response_type={request.ResponseType}. Values: 'code', 'token'."
                        });
                }
            }
        }
    }
}
