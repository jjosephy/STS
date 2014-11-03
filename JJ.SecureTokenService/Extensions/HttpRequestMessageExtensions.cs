using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using TokenRequestEncryption;

namespace JJ.SecureTokenService.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        static readonly TokenCryptoManager crypto = new TokenCryptoManager("localhost");

        public static string DecryptBody(this HttpRequestMessage request, string encrypted)
        {
            return crypto.Decrypt(encrypted);
        }
    }
}