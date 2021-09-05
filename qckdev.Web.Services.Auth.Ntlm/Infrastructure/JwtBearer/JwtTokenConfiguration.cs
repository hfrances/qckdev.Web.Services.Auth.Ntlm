using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Infrastructure.JwtBearer
{
    public class JwtTokenConfiguration
    {

        public string Key { get; set; }
        public double? AccessExpireSeconds { get; set; }

    }
}
