using JJ.SecureTokenService.Contracts;
using JJ.SecureTokenService.Contracts.V1;
using JJ.SecureTokenService.RequestContext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using TokenRequestEncryption;

namespace JJ.SecureTokenService.Extensions
{
    internal static partial class HttpRequestMessageExtensions
    {
        const string RequestVersion = "x-api-version";
        const string RelyingParty = "relyingParty";
        const string CorrelationId = "x-correlation-id";

        internal static string DecryptBody(this HttpRequestMessage request, string encrypted)
        {
            return TokenCryptoManager.Instance.Decrypt(encrypted);
        }

        internal static double GetRequestVersion(this HttpRequestMessage request)
        {
            double version = 0.0;
            var parsed = GetHeaderValue(request.Headers, RequestVersion);
            double.TryParse(parsed, out version);

            return version;
        }

        internal static Guid? GetRequestCorrelationId(this HttpRequestMessage request)
        {
            Guid? correlationId = null;
            var guid = Guid.Empty;
            if (Guid.TryParse(GetHeaderValue(request.Headers, CorrelationId), out guid))
            {
                correlationId = guid;
            }

            return correlationId;
        }

        internal static string GetRelyingParty(this HttpRequestMessage request)
        {
            return GetHeaderValue(request.Headers, RelyingParty);
        }

        internal static async Task<ITokenServiceRequestContext> GetSTSRequestContextAsync(
            this HttpRequestMessage request)
        {
            double version = request.GetRequestVersion();
            Guid? correlationId = request.GetRequestCorrelationId();
            var relyingParth = request.GetRelyingParty();

            var requestBody = default(TokenRequestBase);
            if (request.Method.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                requestBody = await CreateTokenRequestTypeAsync(request, version);
            }

            return new TokenServiceRequestContext(
                relyingParth, 
                version, 
                correlationId == null ? Guid.Empty : correlationId.Value, 
                requestBody);
        }

        private static string GetHeaderValue(HttpRequestHeaders headers, string headerName)
        {
            var values = GetHeaderValues(headers, headerName);
            if (values.Any())
            {
                // TODO: Test this to make sure it works in all cases, like null value of header key that exists
                return values.Take(1).FirstOrDefault();
            }

            return string.Empty;
        }

        private static IEnumerable<string> GetHeaderValues(HttpRequestHeaders headers, string headerName)
        {
            var header = headers.Where(
                fx => fx.Key.Equals(headerName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if ( header.Key != null && header.Value != null )
            {
                return header.Value;
            }

            return new List<string>();
        }

        private static async Task<TokenRequestBase> CreateTokenRequestTypeAsync(
            HttpRequestMessage request,
            double version)
        {
            try
            {
                var body = await request.Content.ReadAsStringAsync();
                if (version == 1.0)
                {
                    return JsonConvert.DeserializeObject<TokenRequestV1>(body);
                }
            }
            catch (Exception ex)
            {
                // TODO: Test this, since crypto could throw, either handle here or change crypto to handle
            }

            // Support other version types here

            return null;
        }
    }
}