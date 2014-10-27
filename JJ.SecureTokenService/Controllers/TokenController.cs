using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace JJ.SecureTokenService.Controllers
{
    public class TokenController : ApiController
    {
        public TokenController()
        {
        }

        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
        }

        /// <summary>
        /// POST to STS to get a Bearer Token
        /// </summary>
        /// <returns>An OAuth2 Bearer Token</returns>
        public async Task<HttpResponseMessage> Post()
        {
            // parse the body
            var body = await this.Request.Content.ReadAsStringAsync();
            var parts = body.Split('&');

            var identity = new ClaimsIdentity(OwinStartUp.OAuthBearerOptions.AuthenticationType);

            // TODO: validate the parts
            identity.AddClaim(new Claim(ClaimTypes.Name, parts[0]));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, parts[1]));
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, parts[2]));

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());

            var currentUtc = new SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;

            // Set expiration
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));
            
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(OwinStartUp.OAuthBearerOptions.AccessTokenFormat.Protect(ticket))
            };
        }
    }
}
