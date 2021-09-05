using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Exceptions
{

    public sealed class UnknownUserException : Exception
    {

        public UnknownUserException()
        {
        }

        public UnknownUserException(string message) : base(message)
        {
        }

        public UnknownUserException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
