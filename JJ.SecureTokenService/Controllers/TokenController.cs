
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using JJ.SecureTokenService.Contracts.V1;
using JJ.SecureTokenService.Extensions;
using JJ.SecureTokenService.HttpResponseMessages;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;

namespace JJ.SecureTokenService.Controllers
{
    public class TokenController : ApiController
    {
        /// <summary>
        /// TODO: make sure to handle this error as it could throw if cert is not found
        /// </summary>

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
                var decrypted = this.Request.DecryptBody(await this.Request.Content.ReadAsStringAsync());
                if (string.IsNullOrWhiteSpace(decrypted))
                {
                    return ServiceResponseMessage.Unauthorized();
                }

                var body = JsonConvert.DeserializeObject<TokenRequestV1>(decrypted);
                if (body == null)
                {
                    return ServiceResponseMessage.Unauthorized();
                }

                // TODO: need to do full user auth
                // Create Identity and Set Claims
                var authType = OwinStartUp.OAuthBearerOptions.AuthenticationType;
                var identity = new ClaimsIdentity(authType);

                // TODO: validate the parts
                identity.AddClaim(new Claim(ClaimTypes.Name, body.UserName));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Email, body.Email));
                identity.AddClaim(new Claim(ClaimTypes.MobilePhone, body.MobilePhone));
                identity.AddClaim(new Claim(ClaimTypes.Authentication, "Partial"));

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
            catch (Exception ex)
            {
                // Test all of these exceptions
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
            finally
            {
                 // Log event
            }
            
        }
    }
}
