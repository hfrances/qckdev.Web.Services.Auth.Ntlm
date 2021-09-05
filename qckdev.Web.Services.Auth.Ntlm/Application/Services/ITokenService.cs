using qckdev.Web.Services.Auth.Ntlm.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Services
{
    public interface ITokenService
    {

        Task<IEnumerable<Token>> GetTokensByWindowsUserAsync(string login);
        Task<Token> SaveTokenToWindowsUserAsync(string login, string type, string value, DateTimeOffset expires);
        Task BanTokenAsync(IEnumerable<Guid> tokenIds);
        Task ConsumeTokenAsync(IEnumerable<Guid> tokenIds);

    }
}
