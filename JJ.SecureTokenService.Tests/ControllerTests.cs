using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using JJ.SecureTokenService.Controllers;
using JJ.SecureTokenService.Contracts.V1;
using OwinSelfHostServer;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace JJ.SecureTokenService.Tests
{
    [TestClass]
    public class ControllerTests
    {
        /// <summary>
        /// Base address for test host
        /// </summary>
        static Uri baseUri = new Uri("http://localhost:8080");

        /// <summary>
        /// OWIN Host for Testing Requests
        /// </summary>
        static Server<OwinStartUp> server;

        /// <summary>
        /// Certificate Validation routine
        /// </summary>
        static Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validateCertificate = ValidateServerCertificate;

        /// <summary>
        /// Cert Subject name for testing certs
        /// </summary>
        const string CertSubjectName = "localhost";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var cert = CertificateManager.GetClientCert(CertSubjectName);
            server = Server<OwinStartUp>.Create(
                baseUri: baseUri, 
                clientCert: cert,
                validateCertificate: validateCertificate);
        }

        [TestMethod]
        public async Task TokenController_TestPostV1()
        {
            var response = await server.CreateRequestAsync<TokenRequestV1>(
                HttpMethod.Post,
                "/token",
                value: CreateRequestBodyV1());

            var token = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, "Status Codes do not match");
            Assert.IsFalse(string.IsNullOrWhiteSpace(token), "Token is an empty string");

        }

        [TestMethod]
        public async Task TokenController_TestPostUnathorizedUserV1()
        {
            var response = await server.CreateRequestAsync<TokenRequestV1>(
                HttpMethod.Post,
                "token",
                value: CreateRequestBodyV1());

            Assert.IsTrue(
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized, 
                "Incorrect Response Code Returned, should be 401");
        }

        public static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // TODO : make this a func that can be passed
            return true;
        }

        private TokenRequestV1 CreateRequestBodyV1()
        {
            return new TokenRequestV1
            {
                Email = "test@test.com",
                RelyingParty = "http://my.nordstrom.com",
                Password = "password",
                AccessToken = Guid.NewGuid().ToString(),
                UserName = "username",
                MobilePhone = "1234567890"
            };
        }
    }
}
