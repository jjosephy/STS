using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JJ.SecureTokenService.Handler
{
    public class CertificateHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var cert = request.GetClientCertificate();
            if (cert != null)
            {
                // Not sure how this ever results in not null. Hope to find out
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}