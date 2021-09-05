using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using qckdev.Web.Services.Auth.Ntlm.Application.Services;
using qckdev.Web.Services.Auth.Ntlm.Persistence.Entities;
using System;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence
{
    public static class DependencyInjections
    {

        public static IServiceCollection AddPersistence(this IServiceCollection service, Action<DbContextOptionsBuilder> optionsAction)
        {
            service
                .AddScoped<ITokenService, TokenService>()
                .AddDbContext<ApplicationDbContext>(optionsAction)
                .AddIdentityCore<ApplicationUser>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
            ;
            return service;
        }

    }
}
