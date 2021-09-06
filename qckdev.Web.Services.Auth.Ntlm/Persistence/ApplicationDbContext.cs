using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;
using System;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence
{
    sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {

        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<OAuth2Token> OAuth2Tokens { get; set; }
        public DbSet<OAuth2TokenProperty> OAuth2TokenProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

    }
}
