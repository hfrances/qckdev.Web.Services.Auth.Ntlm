using Microsoft.EntityFrameworkCore;
using qckdev.Web.Services.Auth.Ntlm.Application;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence
{
    sealed class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProvider> UserProviders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

    }
}
