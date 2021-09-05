using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence.Configuration
{
    public class UserProviderConfiguration : IEntityTypeConfiguration<UserProvider>
    {
        public void Configure(EntityTypeBuilder<UserProvider> builder)
        {

            builder.HasIndex(x => new { x.ProviderName, x.Value });
            builder
                .HasOne(o => o.User)
                .WithMany(n => n.Providers)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
