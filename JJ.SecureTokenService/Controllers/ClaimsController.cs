
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using JJ.SecureTokenService.Contracts.V1;
using Newtonsoft.Json;
using JJ.SecureTokenService.HttpResponseMessages;

namespace JJ.SecureTokenService.Controllers
{
    public class ClaimsController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var identity = this.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return ServiceResponseMessage.BadRequest("Could not parse identity as claims identity");
            }

            var model = ParseClaims(identity.Claims);
            return ServiceResponseMessage.Ok(model);
        }

        private ClaimsResponseV1 ParseClaims(IEnumerable<Claim> claims)
        {
            // TODO: figure out a cleaner way to parse claims
            var email = claims.Where(fx => fx.Type == ClaimTypes.Email);
            var id = claims.Where(fx => fx.Type == ClaimTypes.NameIdentifier);
            var mobilePhone = claims.Where(fx => fx.Type == ClaimTypes.MobilePhone);
            var userName = claims.Where(fx => fx.Type == ClaimTypes.Name);
            var authentication = claims.Where(fx => fx.Type == ClaimTypes.Authentication);

            return new ClaimsResponseV1
            {
                Authentication = !authentication.Any() ? "none" : authentication.First().Value,
                Email = !email.Any() ? string.Empty : email.First().Value,
                MobilePhone = !mobilePhone.Any() ? string.Empty : mobilePhone.First().Value,
                UserId = !id.Any() ? string.Empty : id.First().Value,
                UserName = !userName.Any() ? string.Empty : userName.First().Value
            };
        }
    }
}
