using MediatR;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Queries
{
    public sealed class GetUserQuery : IRequest<UserDto>
    {
    }
}
