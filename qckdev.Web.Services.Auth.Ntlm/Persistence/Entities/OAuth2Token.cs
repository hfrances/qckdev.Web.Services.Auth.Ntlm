using System;
using System.Collections.Generic;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Entities
{
    sealed class OAuth2Token
    {

        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTimeOffset ExpiresUtc { get; set; }
        public bool Banned { get; set; }
        public bool Consumed { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<OAuth2TokenProperty> Properties { get; set; }

    }
}
