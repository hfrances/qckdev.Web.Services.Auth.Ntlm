using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Configuration
{
    sealed class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.HasIndex(x => new { x.Type, x.Value });
            builder
                .HasOne(o => o.User)
                .WithMany(n => n.Tokens)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
