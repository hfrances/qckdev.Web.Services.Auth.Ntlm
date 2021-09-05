using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Exceptions
{
    sealed class OAuth2Exception : Exception
    {

        public OAuth2Exception(OAuth2ErrorDto error)
            : base(error.Description ?? error.Code.ToStringJson())
        {
            this.Error = error;
        }

        public OAuth2Exception(OAuth2ErrorDto error, Exception innerException)
            : base(error.Description ?? error.Code.ToStringJson(), innerException)
        {
            this.Error = error;
        }


        public OAuth2ErrorDto Error { get; }

    }
}
