
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using SecureTokenServiceClient.Models;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.Net.Security;
using System.IO;
using System.Text;
using TokenRequestEncryption;
using System.Net.Http.Formatting;
using SecureTokenServiceClient.MediaFormatter;

namespace SecureTokenServiceClient.Client
{
    public class TokenClient
    {
        const int TimeOut = 50000;
        
        /// <summary>
        /// TODO: make sure to handle this error as it could throw if cert is not found
        /// </summary>
        static readonly TokenCryptoManager crypto = new TokenCryptoManager("localhost");

        //readonly Uri baseUri = new Uri("http://localhost:8088");
        readonly Uri baseUri = new Uri("https://aliex:444");
        readonly HttpClient client;

        public TokenClient()
        {
            var handler = new WebRequestHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(GetClientCert());
            //not sure if this should be set or not
            handler.UseDefaultCredentials = true;
            handler.UseProxy = false;
            ServicePointManager.ServerCertificateValidationCallback = 
                new RemoteCertificateValidationCallback(ValidateServerCertificate);
            //handler.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequired;

            client = new HttpClient(handler)
            {
                BaseAddress = baseUri,
                Timeout = new TimeSpan(0, 0, 0, 0, TimeOut)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static bool ValidateServerCertificate(
            object sender, 
            X509Certificate certificate, 
            X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public async Task<TokenResponseModel> GetTokenAsync(AuthenticationModel authentication)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(authentication);
                var encrypted = crypto.Encrypt(serialized);
                
                var response = await this.client.PostAsync<string>(
                    "/token", 
                    encrypted, 
                    new TextMediaFormatter());

                var tokenResponse = new TokenResponseModel
                {
                    StatusCode = response.StatusCode
                };

                var responseText = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tokenResponse.AuthToken = responseText;
                }
                else
                {
                    tokenResponse.Message = responseText;
                }

                return tokenResponse;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<ClaimsResponseModel> GetClaimsAsync(string token)
        {
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync("/claims");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ClaimsResponseModel>(await response.Content.ReadAsStringAsync());
            }

            return new ClaimsResponseModel();
        }

        private X509Certificate GetClientCert()
        {
            X509Store store = null;
            try
            {
                store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

                var certs = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", true);

                if ( certs.Count == 1 )
                {
                    var cert = certs[0];
                    return cert;
                }
            }
            finally
            {
                if ( store != null )
                    store.Close();
            }

            return null;
        }
    }
}