using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JJ.SecureTokenService.Application;
using JJ.SecureTokenService.Contracts.V1;
using JJ.SecureTokenService.Extensions;
using JJ.SecureTokenService.HttpResponseMessages;
using JJ.SecureTokenService.RequestContext;
using Newtonsoft.Json;
using System.Web.Http;

namespace JJ.SecureTokenService.Handler
{
    public class ValidateRequestHandler : DelegatingHandler
    {
        /// <summary>
        /// List of relying parties that are supported
        /// </summary>
        static HashSet<string> relyingParties = new HashSet<string>()
        {
            "http://jj.sts.test.com",
            "http://jj.tokenclient.com"
        };

        /// <summary>
        /// Versions that this service supports
        /// </summary>
        static HashSet<double> supporttedVersions = new HashSet<double>
        {
            1.0
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new HttpResponseException(
                    new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("HttpRequestMessage is null.")
                    });
            }

            var context = await request.GetSTSRequestContextAsync();
            if (!ValidateContext(context))
            {
                throw new HttpResponseException(ServiceResponseMessage.Unauthorized());
            }

            request.Properties.Add(STSConstants.STSRequestContext, context);

            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Validates the context. In some cases we will provide a nice message, but for the most part we dont 
        /// want to emit detailed error messages to keep things secure. 
        /// </summary>
        /// <param name="context">Current STS Context</param>
        /// <returns>True if valid, false if not</returns>
        private bool ValidateContext(ITokenServiceRequestContext context)
        {
            if (!relyingParties.Contains(context.RelyingParty))
            {
                return false;
            }

            if (!supporttedVersions.Contains(context.Version))
            {
                throw new HttpResponseException(
                    new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                       Content = new StringContent("Invalid Version")
                    });
            }

            if(context.CorrelationId == null || context.CorrelationId.Value.Equals(Guid.Empty))
            {
                throw new HttpResponseException(
                    new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Invalid Correlation Id")
                    });
            }

            return true;
        }
    }
}