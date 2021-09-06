using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using qckdev.Web.Services.Auth.Ntlm.Application.Commands;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Exceptions;
using qckdev.Web.Services.Auth.Ntlm.Application.Helpers;
using qckdev.Web.Services.Auth.Ntlm.Application.Models;
using qckdev.Web.Services.Auth.Ntlm.Application.Services;
using qckdev.Web.Services.Auth.Ntlm.Infrastructure.JwtBearer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Handlers
{
    public sealed class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, TokenDto>
    {

        IHttpContextAccessor HttpContextAccessor { get; }
        IOptionsMonitor<JwtBearerOptions> JwtOptionsMonitor { get; }
        IOptionsMonitor<JwtBearerMoreOptions> JwtMoreOptionsMonitor { get; }
        ITokenService TokenService { get; }

        public CreateTokenCommandHandler(
            IHttpContextAccessor httpContextAccessor, IOptionsMonitor<JwtBearerOptions> jwtOptionsMonitor, IOptionsMonitor<JwtBearerMoreOptions> jwtMoreOptionsMonitor,
            ITokenService tokenService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.JwtOptionsMonitor = jwtOptionsMonitor;
            this.JwtMoreOptionsMonitor = jwtMoreOptionsMonitor;
            this.TokenService = tokenService;
        }

        public async Task<TokenDto> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
        {
            switch (request.GrantType)
            {
                case GrantType.Authorization_Code:

                    try
                    {
                        var tokenCode = await TokenService.GetTokenAsync(Constants.TOKENTYPE_CODE, request.Code);

                        if (tokenCode == null)
                        {
                            throw new OAuth2Exception(new OAuth2ErrorDto
                            {
                                Code = AuthorizationErrorCode.AccessDenied,
                                Description = "The specified authorization_code has not been granted."
                            });
                        }
                        else
                        {
                            var userName = tokenCode.Properties.Single(x=> x.Name == Constants.TOKENPROPERTY_LOGIN)?.Value;
                            var accessTypeString = tokenCode.Properties.SingleOrDefault(x => x.Name == Constants.TOKENPROPERTY_ACCESSTYPE)?.Value;
                            Enum.TryParse(accessTypeString, out AccessType accessType);

                            if (string.IsNullOrWhiteSpace(userName))
                            {
                                throw new OAuth2Exception(new OAuth2ErrorDto
                                {
                                    Code = AuthorizationErrorCode.InvalidRequest,
                                    Description = "Unknown user"
                                });
                            }
                            else if (tokenCode.State == TokenState.Banned)
                            {
                                throw new OAuth2Exception(new OAuth2ErrorDto
                                {
                                    Code = AuthorizationErrorCode.AccessDenied,
                                    Description = "The specified authorization_code is banned."
                                });
                            }
                            else if (tokenCode.State == TokenState.Consumed)
                            {
                                throw new OAuth2Exception(new OAuth2ErrorDto
                                {
                                    Code = AuthorizationErrorCode.AccessDenied,
                                    Description = "The specified authorization_code has already been used."
                                });
                            }
                            else
                            {
                                var jwtTokenOptions = JwtOptionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
                                var jwtTokenMoreOptions = JwtMoreOptionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
                                TokenDto tokenDto;

                                await TokenService.ConsumeTokenAsync(new[] { tokenCode.TokenId });
                                tokenDto = Helper.CreateToken<TokenDto>(jwtTokenOptions, jwtTokenMoreOptions, userName, Array.Empty<Claim>(), accessType);
                                await Helper.UpdateRefreshTokenAsync(this.TokenService, userName, tokenDto.RefreshToken);
                                return tokenDto;
                            }
                        }
                    }
                    catch (OAuth2Exception)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new OAuth2Exception(new OAuth2ErrorDto
                        {
                            Code = AuthorizationErrorCode.InvalidRequest,
                            Description = ex.Message
                        }, ex);
                    }

                case Models.GrantType.Refresh_Token:
                // https://dev.to/moe23/refresh-jwt-with-refresh-tokens-in-asp-net-core-5-rest-api-step-by-step-3en5
                default:
                    throw new OAuth2Exception(new OAuth2ErrorDto
                    {
                        Code = AuthorizationErrorCode.InvalidRequest,
                        Description = $"Invalid value for grant_type={request.GrantType}. Values: 'authorization_code', 'refresh_token'."
                    });
            }
        }
    }
}
