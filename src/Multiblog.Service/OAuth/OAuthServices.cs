using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Multiblog.Core.Model.Setting;
using Multiblog.Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Multiblog.Service.OAuth
{
    public class OAuthServices : IOAuthServices
    {
        private readonly IHttpContextAccessor _context;
        private readonly UrlSettings _urlSetting;
        private DiscoveryResponse _disco = null;
        private TokenClient _tokenClient = null;

        public OAuthServices(IHttpContextAccessor context,
            IOptions<UrlSettings> urlSetting)
        {
            _context = context;
            _urlSetting = urlSetting.Value;
        }

        public async Task ValidateUserAsync(string username, string password)
        {
            // discover endpoints from metadata
            if (_disco == null)
            {
                _disco = await DiscoveryClient.GetAsync(_urlSetting.OAuthServerUrl);
                if (_disco.IsError)
                {
                    Console.WriteLine(_disco.Error);
                        
                    _disco = null;
                    return;
                }
            }

            // request token
            if (_tokenClient == null)
            {
                _tokenClient = new TokenClient(_disco.TokenEndpoint, "MetaWeblog.client", "KanBandyBliKul");
            }

            var tokenResponse = await _tokenClient.RequestResourceOwnerPasswordAsync(username, password, "blog");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                throw new UnauthorizedAccessException("Unauthorized");
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, username));

            var stream = tokenResponse.AccessToken;
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jsonToken = (JwtSecurityToken)handler.ReadToken(stream);

            if (jsonToken?.Claims != null)
            {
                foreach (var item in jsonToken.Claims)
                {
                    identity.AddClaim(item);
                }
            }

            _context.HttpContext.User = new ClaimsPrincipal(identity);
        }
    }
}
