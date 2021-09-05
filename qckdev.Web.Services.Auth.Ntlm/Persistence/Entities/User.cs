using System;
using System.Collections.Generic;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Entities
{
    public sealed class User
    {

        public Guid UserId { get; set; }

        public IEnumerable<UserProvider> Providers { get; set; }
        public IList<Token> Tokens { get; set; }

    }
}
