using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TokenRequestEncryption.Tests
{
    [TestClass]
    public class EncryptionTests
    {
        const string CertSubjectName = "localhost";

        [TestMethod]
        public void TestEncryptDecrypt_SimpleString()
        {
            var token = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var encrypted = TokenCryptoManager.Instance.Encrypt(token);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(encrypted), "Token encrypted value is null or whitespace");

            var decrypted = TokenCryptoManager.Instance.Decrypt(encrypted);
            Assert.IsTrue(decrypted == token, "Decrypted text does not match original");
        }
    }
}
