
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
        /// <summary>
        /// If timeout occurs then the task will be aborted so be prepared for what badness might happen.
        /// </summary>
        const int TimeOut = 500000;

        /// <summary>
        ///  The url of the Token Service. Needs to be configured
        /// </summary>
        readonly Uri baseUri = new Uri("https://localhost:444");

        /// <summary>
        /// Http Client used to fire requests to the service
        /// </summary>
        //readonly HttpClient client;

        /// <summary>
        /// Default Ctor
        /// </summary>
        public TokenClient()
        {
            //client = new HttpClient()
            //{
            //    BaseAddress = baseUri,
            //    Timeout = new TimeSpan(0, 0, 0, 0, TimeOut)
            //};

            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Gets a Token from the Token Service
        /// </summary>
        /// <param name="authentication">The authentication body</param>
        /// <returns></returns>
        public async Task<TokenResponseModel> GetTokenAsync(AuthenticationModel authentication)
        {
            try
            {
                var client = CreateClient();
                var response = await client.PostAsync<string>(
                    "/token", 
                    JsonConvert.SerializeObject(authentication), 
                    new TextMediaFormatter());

                var tokenResponse = new TokenResponseModel
                {
                    StatusCode = response.StatusCode
                };

                var responseText = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new TokenResponseModel
                    {
                        StatusCode = response.StatusCode,
                        AuthToken = responseText
                    };
                }
                else
                {
                    return new TokenResponseModel
                    {
                        StatusCode = response.StatusCode,
                        Message = responseText
                    };
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        /// <summary>
        /// Gets the Claims that are associated with a Token
        /// </summary>
        /// <param name="token">The token to get the claims for</param>
        /// <returns>An awaitable Task that results in a ClaimsResponseModel</returns>
        public async Task<ClaimsResponseModel> GetClaimsAsync(string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            var response = await client.GetAsync("/claims");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ClaimsResponseModel>(await response.Content.ReadAsStringAsync());
            }

            return new ClaimsResponseModel();
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = baseUri,
                Timeout = new TimeSpan(0, 0, 0, 0, TimeOut)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-api-version", "1.0");
            client.DefaultRequestHeaders.Add("x-correlation-id", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("relyingParty", "http://jj.tokenclient.com");

            return client;
        }
    }
}