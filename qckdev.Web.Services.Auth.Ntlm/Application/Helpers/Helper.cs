using Microsoft.AspNetCore.Authentication.JwtBearer;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Models;
using qckdev.Web.Services.Auth.Ntlm.Application.Services;
using qckdev.Web.Services.Auth.Ntlm.Infrastructure.JwtBearer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Helpers
{
    static class Helper
    {

        public const string CLAIM_ACCESSTYPE = "access_type";


        public static TResult CreateToken<TResult>(JwtBearerOptions jwtOptions, JwtBearerMoreOptions jwtMoreOptions, string userName, IEnumerable<Claim> claims, AccessType accessType)
            where TResult : TokenDto, new()
        {
            var key = jwtOptions.TokenValidationParameters.IssuerSigningKey;
            var tmp = JwtGenerator.CreateToken(key, userName, claims: claims, lifespan: jwtMoreOptions.TokenLifeTimespan);

            return new TResult
            {
                AccessToken = tmp.AccessToken,
                TokenType = TokenType.Bearer,
                ExpiresIn = Math.Ceiling((((DateTime)tmp.Expired) - DateTime.UtcNow).TotalSeconds),
                RefreshToken = (accessType == Models.AccessType.Offline ? tmp.RefreshToken : null),
            };
        }

        public async static Task UpdateCodeTokenAsync(ITokenService tokenService, string userName, [DisallowNull] string code, DateTimeOffset expiredUtc)
        {
            var tokens = await tokenService.GetTokensByWindowsUserAsync(userName);
            var pendingTokens = tokens.Where(x => x.Type == Constants.TOKENTYPE_CODE && x.State == TokenState.Active).Select(x => x.TokenId);

            await tokenService.BanTokenAsync(pendingTokens);
            if (code != null)
            {
                // Crear un nuevo refresh_token.
                // TODO: ¿Cuánto debería ser el refresh_token?
                await tokenService.SaveTokenToWindowsUserAsync(userName, Constants.TOKENTYPE_CODE, code, expiredUtc);
            }
        }

        public async static Task UpdateRefreshTokenAsync(ITokenService tokenService, string userName, string refreshToken)
        {
            var tokens = await tokenService.GetTokensByWindowsUserAsync(userName);
            var pendingTokens = tokens.Where(x => x.Type == Constants.TOKENTYPE_REFRESHTOKEN && x.State == TokenState.Active).Select(x => x.TokenId);

            await tokenService.BanTokenAsync(pendingTokens);
            if (refreshToken != null)
            {
                // Crear un nuevo refresh_token.
                // TODO: ¿Cuánto debería ser el refresh_token?
                await tokenService.SaveTokenToWindowsUserAsync(userName, Constants.TOKENTYPE_REFRESHTOKEN, refreshToken, DateTimeOffset.UtcNow.AddDays(10));
            }
        }

        public static string ToStringJson(this AuthorizationErrorCode value)
            => ToStringJson<AuthorizationErrorCode>(value);

        public static string ToStringJson(this TokenType value)
            => ToStringJson<TokenType>(value);

        private static string ToStringJson<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            var prop = typeof(TEnum).GetField(value.ToString());
            string rdo = null;

            if (prop != null)
            {
                var attr = prop.GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: false)
                    .OfType<JsonPropertyNameAttribute>()
                    .SingleOrDefault();

                rdo = attr?.Name ?? prop.Name;
            }
            if (rdo == null)
            {
                rdo = value.ToString();
            }
            return rdo;
        }

    }
}
