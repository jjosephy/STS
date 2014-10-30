using JJ.SecureTokenService.Contracts.V1;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
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
            try
            {
                // parse the body
                var body = JsonConvert.DeserializeObject<TokenRequestV1>(
                    await this.Request.Content.ReadAsStringAsync());

                // authenticate user here.
                if (body.UserName != "username" || body.Password != "password")
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("User is unathorized")
                    };
                }

                // Create Identity and Set Claims
                var authType = OwinStartUp.OAuthBearerOptions.AuthenticationType;
                var identity = new ClaimsIdentity(authType);

                // use this to set to bearer - this will set IsAuthenticated to true
                // OwinStartUp.OAuthBearerOptions.AuthenticationType;

                // TODO: validate the parts
                identity.AddClaim(new Claim(ClaimTypes.Name, body.UserName));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Email, body.Email));
                identity.AddClaim(new Claim(ClaimTypes.MobilePhone, body.MobilePhone));

                var properties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                var ticket = new AuthenticationTicket(identity, properties);

                var currentUtc = new SystemClock().UtcNow;
                ticket.Properties.IssuedUtc = currentUtc;

                // Set expiration
                ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(OwinStartUp.OAuthBearerOptions.AccessTokenFormat.Protect(ticket))
                };
            }
            catch
            {
                // Test all of these exceptions
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Message Details need to go here")
                };
            }
            finally
            {
                 // Log event
            }
            
        }
    }
}
