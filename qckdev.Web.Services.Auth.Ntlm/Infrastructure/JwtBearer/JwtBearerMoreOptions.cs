using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

namespace qckdev.Web.Services.Auth.Ntlm.Infrastructure.JwtBearer
{
    public class JwtBearerMoreOptions
    {

        public TimeSpan? TokenLifeTimespan { get; set; }

    }
}
