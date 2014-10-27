
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;

namespace OwinSelfHostTestServer
{
    /// <summary>
    /// Class that serves as an in memory host server as well as provides client hooks to 
    /// fire requests through the server.
    /// </summary>
    public class TestHost<T> : IDisposable
    {
        const string TempApiServer = "http://www.TemporaryTestServer.com";
        const string ApiVersion = "x-api-version";

        readonly TestServer server;
        readonly HttpClient client;

        public TestHost() :
            this(TempApiServer, null)
        {
        }

        public TestHost(Type[] types) :
            this(TempApiServer, types)
        {
        }

        public TestHost(string uri, Type[] types)
        {
            this.server = TestServer.Create<T>();
            this.client = new HttpClient(this.server.Handler)
            {
                BaseAddress = new Uri(uri)
            };
            TestAssemblyResolver.RegisterTypes(types);
        }

        public async Task<HttpResponseMessage> CreateRequestAsync<T>(
            HttpMethod method, 
            string uri,
            string authToken = "",
            T value = default(T),
            uint version = 1)
        {
            client.DefaultRequestHeaders.Add(ApiVersion, version.ToString());
            
            // For now always add bearer token, change this later
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);

            if (method == HttpMethod.Get)
            {
                return await client.GetAsync(uri);
            }
            else if (method == HttpMethod.Post)
            {
                return await client.PostAsJsonAsync<T>(uri, value);
            }
            else if (method == HttpMethod.Delete)
            {
                return await client.DeleteAsync(uri);
            }

            throw new NotSupportedException("Method not supported");
        }

        public void Dispose()
        {
            if (server != null)
            {
                server.Dispose();
            }

            if (client != null)
            {
                client.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
