using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Configuration
{
    sealed class OAuth2TokenPropertyConfiguration : IEntityTypeConfiguration<OAuth2TokenProperty>
    {

        public void Configure(EntityTypeBuilder<OAuth2TokenProperty> builder)
        {
            builder.HasKey(x => x.TokenPropertyId);
            builder.HasIndex(x => new { x.TokenId, x.Name, x.Value }).IsUnique();

            builder
                .HasOne(x => x.Token)
                .WithMany(x => x.Properties)
                .HasForeignKey(fk => fk.TokenId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
