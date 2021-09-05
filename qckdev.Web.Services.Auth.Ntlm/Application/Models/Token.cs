using System;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Models
{
    public sealed class Token
    {
        public Guid TokenId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTimeOffset ExpiresUtc { get; set; }
        public TokenState State { get; set; }

    }
}
