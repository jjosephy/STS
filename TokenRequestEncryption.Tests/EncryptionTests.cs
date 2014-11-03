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
            var crypto = new TokenCryptoManager(CertSubjectName);
            var encrypted = crypto.Encrypt(token);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(encrypted), "Token encrypted value is null or whitespace");

            var decrypted = crypto.Decrypt(encrypted);
            Assert.IsTrue(decrypted == token, "Decrypted text does not match original");
        }

        [TestMethod]
        public void TestEncrypt_EmtpySubjectName()
        {
            try
            {
                var crypto = new TokenCryptoManager(string.Empty);
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message == "subjectName");
            }
        }

        [TestMethod]
        public void TestEncrypt_CertNotFound()
        {
            try
            {
                var crypto = new TokenCryptoManager("CertNotFound");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message == "certificate not found by subject name");
            }
        }
    }
}
