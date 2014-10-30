
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using SecureTokenServiceClient.Models;
using Newtonsoft.Json;

namespace SecureTokenServiceClient.Client
{
    public class TokenClient
    {
        const int TimeOut = 2500;
        readonly Uri baseUri = new Uri("http://localhost:8088");
        readonly HttpClient client;

        public TokenClient()
        {
            client = new HttpClient
            {
                BaseAddress = baseUri,
                Timeout = new TimeSpan(0, 0, 0, 0, TimeOut)
            };
        }

        public async Task<TokenResponseModel> GetTokenAsync(AuthenticationModel authentication)
        {
            var response = await this.client.PostAsJsonAsync<AuthenticationModel>("/token", authentication);

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
    }
}