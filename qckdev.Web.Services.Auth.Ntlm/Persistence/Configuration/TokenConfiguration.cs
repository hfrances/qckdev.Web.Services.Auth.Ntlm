using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Configuration
{
    sealed class TokenConfiguration : IEntityTypeConfiguration<OAuth2Token>
    {
        public void Configure(EntityTypeBuilder<OAuth2Token> builder)
        {
            builder.HasKey(x => x.TokenId);
            builder.HasIndex(x => new { x.Type, x.Value });
            builder
                .HasOne(o => o.User)
                .WithMany(n => n.Tokens)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
