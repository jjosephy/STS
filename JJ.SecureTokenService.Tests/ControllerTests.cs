using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwinSelfHostTestServer;
using System.Net.Http;
using System.Threading.Tasks;
using JJ.SecureTokenService.Controllers;

namespace JJ.SecureTokenService.Tests
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        public async Task TokenController_TestPost()
        {
           var host = new TestHost<OwinStartUp>();
           var response = await host.CreateRequestAsync<string>(
                HttpMethod.Post,
                "api/token",
                value: "here");
            var token = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(token), "Token is an empty string");
        }
    }
}
