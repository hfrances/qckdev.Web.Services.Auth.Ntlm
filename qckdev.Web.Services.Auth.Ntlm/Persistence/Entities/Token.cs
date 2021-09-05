using System;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Entities
{
    public sealed class Token
    {

        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTimeOffset ExpiresUtc { get; set; }
        public bool Banned { get; set; }
        public bool Consumed { get; set; }

        public User User { get; set; }

    }
}
