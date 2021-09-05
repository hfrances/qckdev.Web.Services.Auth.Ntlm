using MediatR;
using Microsoft.AspNetCore.Http;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Exceptions;
using qckdev.Web.Services.Auth.Ntlm.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Handlers
{
    public sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    {

        IHttpContextAccessor HttpContextAccessor { get; }

        public GetUserQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = HttpContextAccessor.HttpContext.User;
            var userName = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new UnknownUserException("User not found");
            }
            else
            {
                return await Task.FromResult(new UserDto
                {
                    UserName = userName,
                });
            }
        }
    }
}
