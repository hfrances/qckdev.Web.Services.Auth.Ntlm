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
                case Models.GrantType.Authorization_Code:
                    var jwtCodeOptions = JwtOptionsMonitor.Get(Constants.AUTHENTICATIONSCHEME_CODE);

                    try
                    {
                        var code = JwtGenerator.ValidateToken(jwtCodeOptions.TokenValidationParameters, request.Code.Substring(2));
                        var userName = code.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId)?.Value;
                        Enum.TryParse(code.Claims.SingleOrDefault(x => x.Type == Helper.CLAIM_ACCESSTYPE)?.Value, out AccessType accessType);
                        var newClaims = code.Claims.Where(x => x.Type != JwtRegisteredClaimNames.NameId && x.Type != Helper.CLAIM_ACCESSTYPE);

                        if (string.IsNullOrWhiteSpace(userName))
                        {
                            throw new OAuth2Exception(new OAuth2ErrorDto
                            {
                                Code = AuthorizationErrorCode.InvalidRequest,
                                Description = "Unknown user"
                            });
                        }
                        else
                        {
                            var tokens = await TokenService.GetTokensByWindowsUserAsync(userName);
                            var tokenCode = tokens?.SingleOrDefault(x => x.Type == Constants.TOKENTYPE_CODE && x.Value == request.Code);

                            if (tokenCode == null)
                            {
                                throw new OAuth2Exception(new OAuth2ErrorDto
                                {
                                    Code = AuthorizationErrorCode.AccessDenied,
                                    Description = "The specified authorization_code has not been granted."
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
                                tokenDto = Helper.CreateToken<TokenDto>(jwtTokenOptions, jwtTokenMoreOptions, userName, newClaims, accessType);
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
