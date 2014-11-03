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
            var c = request.GetRequestContext().ClientCertificate;
            var cert = request.GetClientCertificate();
            if (cert != null)
            {
                if ( cert.Subject.Contains("Some Name you are expecting"))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(cert.Subject), new[] { "Administrators" });
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}