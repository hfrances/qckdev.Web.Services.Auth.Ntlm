using Microsoft.EntityFrameworkCore;
using qckdev.Web.Services.Auth.Ntlm.Application.Services;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;
using Models = qckdev.Web.Services.Auth.Ntlm.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence
{
    sealed class TokenService : ITokenService
    {

        ApplicationDbContext Context { get; }

        public TokenService(ApplicationDbContext context)
        {
            this.Context = context;
        }

        public async Task<IEnumerable<Models.Token>> GetTokensByWindowsUserAsync(string login)
            => (await GetWindowsUserAsync(Context.Users, login))?.Tokens.Select(MapToken) ?? Array.Empty<Models.Token>();

        public async Task<Models.Token> SaveTokenToWindowsUserAsync(string login, string type, string value, DateTimeOffset expires)
        {
            Token token;
            var user = await CreateOrGetWindowsUserAsync(Context.Users, login);

            token = new Token
            {
                Type = type,
                Value = value,
                ExpiresUtc = expires
            };
            if (user.Tokens == null)
            {
                user.Tokens = new[] { token };
            }
            else
            {
                user.Tokens.Add(token);
            }
            await Context.SaveChangesAsync();
            return MapToken(token);
        }

        public async Task BanTokenAsync(IEnumerable<Guid> tokenIds)
        {
            foreach (var tokenId in tokenIds)
            {
                var token = await Context.Tokens.FindAsync(tokenId);

                token.Banned = true;
            }
            await Context.SaveChangesAsync();
        }

        public async Task ConsumeTokenAsync(IEnumerable<Guid> tokenIds)
        {
            foreach (var tokenId in tokenIds)
            {
                var token = await Context.Tokens.FindAsync(tokenId);

                token.Consumed = true;
            }
            await Context.SaveChangesAsync();
        }

        private static Models.Token MapToken(Token token)
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


        private static async Task<User> GetWindowsUserAsync(IQueryable<User> users, string login)
            => await
                users
                    .Include(i => i.Providers)
                    .Include(i => i.Tokens)
                .SingleOrDefaultAsync(x => x.Providers.Any(y => y.ProviderName == "Windows" && y.Value == login));

        private static async Task<User> CreateOrGetWindowsUserAsync(DbSet<User> users, string login)
        {
            var rdo = await GetWindowsUserAsync(users, login);

            if (rdo == null)
            {
                rdo = new User
                {
                    Providers = new[]{ new UserProvider
                    {
                        ProviderName = "Windows",
                        Value = login
                    }}
                };
                await users.AddAsync(rdo);
            }
            return rdo;
        }

    }
}
