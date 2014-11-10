using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OwinSelfHostServer
{
    public class Server<T>
    {
        const string RequestVersionHeader = "x-api-version";
        const string AuthorizationHeaderTokenName = "Bearer";
        const string CorrelationIdHeader = "x-correlation-id";
        const string RelyingPartyHeader = "relyingParty";

        readonly Uri baseUri;
        readonly int timeOut;
        readonly Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validateFunc;
        readonly X509Certificate clientCert;

        private Server(
            Uri baseUri, 
            X509Certificate clientCert, 
            int timeOut,
            Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validateCertificate)
        {
            this.validateFunc = validateCertificate;
            this.baseUri = baseUri;
            this.timeOut = timeOut;
            this.clientCert = clientCert;
        }

        public static Server<T> Create(
            Uri baseUri, 
            X509Certificate clientCert = null,
            int clientTimeout = 60000,
            Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validateCertificate = null)
        {
            WebApp.Start<T>(url: baseUri.AbsoluteUri);
            return new Server<T>(
                baseUri: baseUri,
                clientCert: clientCert,
                timeOut: clientTimeout,
                validateCertificate: validateCertificate);
        }

        public async Task<HttpResponseMessage> CreateRequestAsync<TBody>(
            HttpMethod method,
            string uri,
            string authToken = "",
            TBody value = default(TBody),
            double version = 1.0,
            string contentType = "text/plain",
            string versionHeaderName = RequestVersionHeader,
            string authorizationHeaderName = AuthorizationHeaderTokenName,
            string relyingParty = "http://sts.com",
            Guid? correlationId = null)
        {
            var handler = new WebRequestHandler();

            if (this.clientCert != null)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.Add(clientCert);
                handler.UseDefaultCredentials = true;
                handler.UseProxy = false;

                if (this.validateFunc != null)
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(this.validateFunc);
                }
            }

            using (var client = new HttpClient(handler)
            {
                BaseAddress = baseUri,
                Timeout = new TimeSpan(0, 0, 0, 0, timeOut)
            })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add(versionHeaderName, version.ToString());
                client.DefaultRequestHeaders.Add(RelyingPartyHeader, relyingParty);
                client.DefaultRequestHeaders.Add(CorrelationIdHeader,
                    correlationId == null ? Guid.Empty.ToString() : correlationId.Value.ToString());

                if (!string.IsNullOrWhiteSpace(authToken))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(authorizationHeaderName, authToken);
                }

                if (method == HttpMethod.Get)
                {
                    return await client.GetAsync(uri);
                }
                else if ( method == HttpMethod.Post )
                {
                    if (contentType == "application/json")
                    {
                        return await client.PostAsJsonAsync<TBody>(uri, value);
                    }
                    else
                    {
                        return await client.PostAsync<TBody>(uri, value, new TextMediaFormatter());
                    }

                }
                else if (method == HttpMethod.Delete)
                {
                    return await client.DeleteAsync(uri);
                }
            }

            throw new NotSupportedException("Method not supported");
        }
    }
}
