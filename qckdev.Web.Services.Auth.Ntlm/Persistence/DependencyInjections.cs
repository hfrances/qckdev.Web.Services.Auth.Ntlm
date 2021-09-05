using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using qckdev.Web.Services.Auth.Ntlm.Application.Services;

namespace qckdev.Web.Services.Auth.Ntlm.Persistence
{
    public static class DependencyInjections
    {

        public static IServiceCollection AddPersistence(this IServiceCollection service)
        {
            service
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("ntlm-auth");
                })
                .AddScoped<ITokenService, TokenService>();
            return service;
        }

    }
}
