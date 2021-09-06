using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Entities
{
    sealed class OAuth2TokenProperty
    {

        public Guid TokenPropertyId { get; set; }
        public Guid TokenId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        
        public OAuth2Token Token { get; set; }

    }
}
