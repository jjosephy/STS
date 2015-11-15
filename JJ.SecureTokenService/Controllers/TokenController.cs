
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using JJ.SecureTokenService.Application;
using JJ.SecureTokenService.Contracts.V1;
using JJ.SecureTokenService.HttpResponseMessages;
using JJ.SecureTokenService.RequestContext;
using JJ.SecureTokenService.Services;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;

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
        /// POST to STS to get an OAuth Token
        /// </summary>
        /// <returns>An OAuth2 Token</returns>
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

                if (requestContext.Version == 1.0)
                {
                    var body = requestContext.TokenRequestBody as TokenRequestV1;
                    if (body == null)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("Could not parse request body")
                        };
                    }

                    //identity.AddClaim(new Claim(ClaimTypes.Name, body.UserName));
                    //identity.AddClaim(new Claim(ClaimTypes.Email, body.Email));
                    //identity.AddClaim(new Claim(ClaimTypes.MobilePhone, body.MobilePhone));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

                    var auth = ServiceLocator.Instance.GetAuthenticationProvider(body.AuthenticationProvider);
                    if (auth == null)
                    {
                        return ServiceResponseMessage.BadRequest("Invalid Authentication Provider passed");
                    }

                    // Call the auth provider here
                    if (!body.AuthenticationProvider.Equals("none", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!await auth.IsAuthenticatedAsync(body.Email, body.Password))
                        {
                            return ServiceResponseMessage.Unauthorized();
                        } 
                        else
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Authentication, "Full"));
                        }
                    }
                }

                var ticket = new AuthenticationTicket(identity, new AuthenticationProperties
                {
                    IsPersistent = false
                });
                var currentUtc = new SystemClock().UtcNow;
                ticket.Properties.IssuedUtc = currentUtc;
                ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));

                var token = OwinStartUp.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
                var bytes = Encoding.UTF8.GetBytes(token);
                var encoded = "NORD " + Convert.ToBase64String(bytes);

                return ServiceResponseMessage.Ok(encoded);
            }
            catch (Exception ex)
            {
                // Test all of these exceptions
                return ServiceResponseMessage.InternalServerError(ex.Message);
            }
            finally
            {
                 // Log event
            }
        }
    }
}
