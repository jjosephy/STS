using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using JJ.SecureTokenService.Controllers;
using JJ.SecureTokenService.Contracts.V1;
using OwinSelfHostServer;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using TokenRequestEncryption;
using Newtonsoft.Json;

namespace JJ.SecureTokenService.Tests
{
    [TestClass]
    public class ControllerTests
    {
        /// <summary>
        /// Base address for test host. 
        /// TODO: Figure out how to run OWIN under https
        /// </summary>
        static Uri baseUri = new Uri("http://localhost:8085");

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

        /// <summary>
        /// Test Relying Party.
        /// </summary>
        const string RelyingParth = "http://jj.sts.test.com";


        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            //var cert = CertificateManager.GetClientCert(CertSubjectName);
            server = Server<OwinStartUp>.Create(
                baseUri: baseUri, 
                clientCert: null,
                validateCertificate: validateCertificate);
        }

        [TestMethod]
        public async Task TokenController_TestPost_V1()
        {
            Guid correlationId = Guid.NewGuid();
            var body = CreateRequestBodyV1();
            var response = await server.CreateRequestAsync<TokenRequestV1>(
                HttpMethod.Post,
                "/token",
                contentType: "application/json",
                value: body,
                relyingParty: RelyingParth,
                correlationId: correlationId);

            var token = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, "Status Codes do not match");
            Assert.IsFalse(string.IsNullOrWhiteSpace(token), "Token is an empty string");
        }

        [TestMethod]
        public async Task TokenController_TestGet_V1()
        {
            Guid correlationId = Guid.NewGuid();
            var response = await server.CreateRequestAsync<TokenRequestV1>(
                HttpMethod.Get,
                "/token",
                contentType: "application/json",
                relyingParty: RelyingParth,
                correlationId: correlationId);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed, "Status Codes do not match");
        }
    
        [TestMethod]
        public async Task TokenController_TestUnauthorizedRequest_V1()
        {
            var body = CreateUnauthenticatedRequestBodyV1();
            var response = await server.CreateRequestAsync<TokenRequestV1>(
                HttpMethod.Post,
                "/token",
                contentType: "application/json",
                value: body,
                relyingParty: RelyingParth,
                correlationId: Guid.NewGuid());

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized, "Status Codes do not match");
        }

        [TestMethod]
        public async Task TokenController_TestPostInvalidCorrelationId_V1()
        {
            var response = await server.CreateRequestAsync<string>(
                HttpMethod.Post,
                "token",
                value: JsonConvert.SerializeObject(CreateRequestBodyV1()),
                relyingParty: RelyingParth,
                correlationId: null);

            Assert.IsTrue(
                response.StatusCode == System.Net.HttpStatusCode.BadRequest, 
                "Incorrect Response Code Returned, should be 400");
        }

        [TestMethod]
        public async Task TokenController_TestPostInvalidVersion_V1()
        {
            var response = await server.CreateRequestAsync<string>(
                HttpMethod.Post,
                "token",
                version: 0,
                value: JsonConvert.SerializeObject(CreateRequestBodyV1()),
                relyingParty: RelyingParth,
                correlationId: Guid.NewGuid());

            Assert.IsTrue(
                response.StatusCode == System.Net.HttpStatusCode.BadRequest,
                "Incorrect Response Code Returned, should be 400");
        }

        [TestMethod]
        public async Task TokenController_TestPostInvalidRelyingParty_V1()
        {
            var response = await server.CreateRequestAsync<string>(
                HttpMethod.Post,
                "token",
                value: JsonConvert.SerializeObject(CreateRequestBodyV1()),
                relyingParty: RelyingParth,
                correlationId: null);

            Assert.IsTrue(
                response.StatusCode == System.Net.HttpStatusCode.BadRequest,
                "Incorrect Response Code Returned, should be 400");
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

        /// <summary>
        /// Encrypts a Request Body
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private string CreateEncryptedString<TBody>(TBody value)
        {
            var body = JsonConvert.SerializeObject(value);
            return TokenCryptoManager.Instance.Encrypt(body);
        }

        private TokenRequestV1 CreateRequestBodyV1()
        {
            return new TokenRequestV1
            {
                AuthenticationProvider = "TestProvider",
                Email = "test@test.com",
                Password = "password",
                ApiKey = Guid.NewGuid().ToString(),
                UserName = "username",
                MobilePhone = "1234567890"
            };
        }

        private TokenRequestV1 CreateUnauthenticatedRequestBodyV1()
        {
            return new TokenRequestV1
            {
                AuthenticationProvider = "TestProvider",
                Email = "unauthorized@test.com",
                Password = "password",
                ApiKey = Guid.NewGuid().ToString(),
                UserName = "username",
                MobilePhone = "1234567890"
            };
        }
    }
}
