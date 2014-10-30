using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwinSelfHostTestServer;
using System.Net.Http;
using System.Threading.Tasks;
using JJ.SecureTokenService.Controllers;
using JJ.SecureTokenService.Contracts.V1;

namespace JJ.SecureTokenService.Tests
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        public async Task TokenController_TestPostV1()
        {
            var body = new TokenRequestV1
            {
                Email = "test@test.com",
                RelyingParty = "http://my.nordstrom.com",
                Password = "password",
                AccessToken = Guid.NewGuid().ToString(),
                UserName = "username",
                MobilePhone = "1234567890"
            };

            var host = new TestHost<OwinStartUp>();
            var response = await host.CreateRequestAsync<TokenRequestV1>(
                HttpMethod.Post,
                "/token",
                value: body);

            var token = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, "Status Codes do not match");
            Assert.IsFalse(string.IsNullOrWhiteSpace(token), "Token is an empty string");
        }

        [TestMethod]
        public async Task TokenController_TestPostUnathorizedUserV1()
        {
            var body = new TokenRequestV1
            {
                Email = "test@test.com",
                RelyingParty = "http://my.nordstrom.com",
                Password = "password",
                AccessToken = Guid.NewGuid().ToString(),
                UserName = "unauthorized",
                MobilePhone = "1234567890"
            };

            // Host make reusable
            var host = new TestHost<OwinStartUp>();
            var response = await host.CreateRequestAsync<TokenRequestV1>(
                HttpMethod.Post,
                "token",
                value: body);

            Assert.IsTrue(
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized, 
                "Incorrect Response Code Returned, should be 401");
        }
    }
}
