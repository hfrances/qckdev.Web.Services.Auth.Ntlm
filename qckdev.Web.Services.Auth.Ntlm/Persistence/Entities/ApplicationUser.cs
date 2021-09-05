using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Entities
{
    sealed class ApplicationUser : IdentityUser<Guid>
    {

        public IList<OAuth2Token> Tokens { get; set; }

    }
}
