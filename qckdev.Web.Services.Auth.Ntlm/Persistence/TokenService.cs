using qckdev.Web.Services.Auth.Ntlm.Application.Services;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;
using Models = qckdev.Web.Services.Auth.Ntlm.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using qckdev.Web.Services.Auth.Ntlm.Application.Exceptions;
using System.DirectoryServices.AccountManagement;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence
{
    sealed class TokenService : ITokenService
    {

        const string LOGINPROVIDER_WINDOWS = "Windows";

        ApplicationDbContext Context { get; }
        UserManager<ApplicationUser> UserManager { get; }

        public TokenService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.Context = context;
            this.UserManager = userManager;
        }

        public async Task<IEnumerable<Models.Token>> GetTokensByWindowsUserAsync(string login)
        {
            IEnumerable<OAuth2Token> rdo;
            var user = await UserManager.FindByLoginAsync(LOGINPROVIDER_WINDOWS, login);

            if (user == null)
            {
                rdo = Array.Empty<OAuth2Token>();
            }
            else
            {
                rdo = Context.OAuth2Tokens.Where(x => x.UserId == user.Id);
            }
            return rdo.Select(MapToken);
        }

        public async Task<Models.Token> SaveTokenToWindowsUserAsync(string login, string type, string value, DateTimeOffset expires)
        {
            OAuth2Token token;
            var user = await CreateOrGetWindowsUserAsync(this.UserManager, login);

            token = new OAuth2Token
            {
                UserId = user.Id,
                Type = type,
                Value = value,
                ExpiresUtc = expires
            };
            await Context.OAuth2Tokens.AddAsync(token);
            await Context.SaveChangesAsync();
            return MapToken(token);
        }

        public async Task BanTokenAsync(IEnumerable<Guid> tokenIds)
        {
            foreach (var tokenId in tokenIds)
            {
                var token = await Context.OAuth2Tokens.FindAsync(tokenId);

                token.Banned = true;
            }
            await Context.SaveChangesAsync();
        }

        public async Task ConsumeTokenAsync(IEnumerable<Guid> tokenIds)
        {
            foreach (var tokenId in tokenIds)
            {
                var token = await Context.OAuth2Tokens.FindAsync(tokenId);

                token.Consumed = true;
            }
            await Context.SaveChangesAsync();
        }

        private static Models.Token MapToken(OAuth2Token token)
            => new()
            {
                TokenId = token.TokenId,
                Type = token.Type,
                Value = token.Value,
                ExpiresUtc = token.ExpiresUtc,
                State =
                    token.Banned ?
                        Models.TokenState.Banned :
                        token.Consumed ?
                            Models.TokenState.Consumed :
                            Models.TokenState.Active,
            };


        private static async Task<ApplicationUser> CreateOrGetWindowsUserAsync(UserManager<ApplicationUser> userManager, string login)
        {
            var user = await userManager.FindByLoginAsync(LOGINPROVIDER_WINDOWS, login);

            if (user == null)
            {
                IdentityResult result;

                user = CreateUserFromPrincipal(login);
                result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider: LOGINPROVIDER_WINDOWS, providerKey: login, displayName: login));
                    if (!result.Succeeded)
                    {
                        throw new UserException(result.Errors.First().Description);
                    }
                }
                else
                {
                    throw new UserException(result.Errors.First().Description);
                }
            }
            return user;
        }

        private static ApplicationUser CreateUserFromPrincipal(string login)
        {
            Guid userId = Guid.NewGuid();
            var domainContext = new PrincipalContext(ContextType.Machine);
            var userPrincipal = UserPrincipal.FindByIdentity(domainContext, login);
            var loginParts = login.Split('\\');

            return new ApplicationUser
            {
                Id = userId,
                UserName = $"{loginParts[1]}@{loginParts[0]}",
                Email = userPrincipal.EmailAddress
            };
        }

    }
}
