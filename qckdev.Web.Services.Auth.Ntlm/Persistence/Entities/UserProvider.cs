using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Entities
{
    public sealed class UserProvider
    {

        public Guid UserProviderId { get; set; }
        public Guid UserId { get; set; }
        public string ProviderName { get; set; }
        public string Value { get; set; }

        public User User { get; set; }

    }
}
