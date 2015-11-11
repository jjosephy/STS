
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
using JJ.SecureTokenService.Application;
using JJ.SecureTokenService.RequestContext;
using JJ.SecureTokenService.Contracts;
using TokenRequestEncryption;

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
                var requestContext = this.Request.Properties[STSConstants.STSRequestContext] as ITokenServiceRequestContext;
                if (requestContext == null)
                {
                    throw new HttpResponseException(ServiceResponseMessage.BadRequest("STS Context is null"));
                }

                // Create Identity and Set Claims
                var authType = OwinStartUp.OAuthBearerOptions.AuthenticationType;
                var identity = new ClaimsIdentity(authType);

                /// LEFTOFF - make this code cleaner, create type by version (type converter). Also take authprovider, 
                /// start building simple factory and inject providers
                if (requestContext.Version == 1.0)
                {
                    // push auth rules to plug ins and take a provider/default provider and pass provider in request

                    var body = requestContext.TokenRequestBody as TokenRequestV1;
                    // TODO: validate the parts
                    identity.AddClaim(new Claim(ClaimTypes.Name, body.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Email, body.Email));
                    identity.AddClaim(new Claim(ClaimTypes.MobilePhone, body.MobilePhone));
                    identity.AddClaim(new Claim(ClaimTypes.Authentication, "Partial"));
                }

                var properties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                var ticket = new AuthenticationTicket(identity, properties);

                var currentUtc = new SystemClock().UtcNow;
                ticket.Properties.IssuedUtc = currentUtc;

                // Set expiration
                ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));

                // TODO: ticket needs to be signed with a certificate. Create new signing cert and use that same cert of other services.
                //TokenCryptoManager.Instance.Decrypt(
                var token = OwinStartUp.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
                //var content = new StringContent(TokenCryptoManager.Instance.Encrypt(token));

                var jwt = new TokenResponseV1
                {
                    Token = token, 
                    AuthLevel = "full"
                };

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(jwt))
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
