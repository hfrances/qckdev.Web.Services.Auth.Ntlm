using MediatR;
using Microsoft.AspNetCore.Mvc;
using qckdev.Web.Services.Auth.Ntlm.Application.Dto;
using qckdev.Web.Services.Auth.Ntlm.Application.Models;

namespace qckdev.Web.Services.Auth.Ntlm.Application.Commands
{
    public sealed class CreateTokenCommand : IRequest<TokenDto>
    {

        public string Code { get; set; }
        [ModelBinder(Name = "username")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [ModelBinder(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [ModelBinder(Name = "client_id")]
        public string ClientId { get; set; }

        [ModelBinder(Name = "client_secret")]
        public string ClientSecret { get; set; }

        [ModelBinder(Name = "redirect_uri")]
        public string RedirectUri { get; set; }

        [ModelBinder(Name = "grant_type")]
        public GrantType GrantType { get; set; }

    }
}
