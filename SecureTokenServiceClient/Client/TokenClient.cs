
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

namespace SecureTokenServiceClient.Client
{
    public class TokenClient
    {
        const int TimeOut = 2500;
        readonly Uri baseUri = new Uri("https://aliex:444");
        readonly HttpClient client;

        public TokenClient()
        {
            
            var handler = new WebRequestHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(GetClientCert());
            //not sure if this should be set or not
            //handler.UseDefaultCredentials = true;
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
            //if ( sslPolicyErrors == SslPolicyErrors.None )
            //    return true;
            //else
            //{
            //    return false;
            //}

            return true;
        }

        public async Task<TokenResponseModel> GetTokenAsync(AuthenticationModel authentication)
        {
            try
            {
                //HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("http://aliex:8088/token");
                //Request.ClientCertificates.Add(GetClientCert());
                //Request.UserAgent = "sample";
                //Request.Method = "POST";

                //UTF8Encoding encoding = new UTF8Encoding();
                //byte[] byte1 = encoding.GetBytes(JsonConvert.SerializeObject(authentication));

                //// Set the content type of the data being posted.
                //Request.ContentType = "application/x-www-form-urlencoded";

                //// Set the content length of the string being posted.
                //Request.ContentLength = byte1.Length;

                //Stream newStream = Request.GetRequestStream();

                //newStream.Write(byte1, 0, byte1.Length);

                //HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                
                //// Get the certificate data.
                //using (var reader = new StreamReader(Response.GetResponseStream()))
                //{
                //    var str = await reader.ReadToEndAsync();
                //}

                var response = await this.client.GetAsync("/claims");

                 response = await this.client.PostAsJsonAsync<AuthenticationModel>("/token", authentication);

                var tokenResponse = new TokenResponseModel
                {
                    StatusCode = response.StatusCode
                };

                var responseText = await response.Content.ReadAsStringAsync();

                if ( response.StatusCode == HttpStatusCode.OK )
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