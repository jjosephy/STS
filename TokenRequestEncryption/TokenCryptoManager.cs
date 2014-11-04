using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TokenRequestEncryption
{
    /// <summary>
    /// TODO: make this a singleton
    /// </summary>
    public sealed class TokenCryptoManager
    {
        /// <summary>
        /// This needs to be configured. Also tested if it doesnt exist, lots of dragons lie within
        /// </summary>
        private const string SubjectName = "localhost";
        private static volatile TokenCryptoManager instance;
        private static object syncRoot = new Object();

        readonly X509Certificate2 certificate;
        readonly RSACryptoServiceProvider encryptProvidor;
        readonly RSACryptoServiceProvider decryptProvidor;

        /// <summary>
        /// TODO: an exception will be thrown if certificate private keys are null should protect that
        /// </summary>
        /// <param name="certSubjectName"></param>
        private TokenCryptoManager()
        {
            this.certificate = GetX509Certificate(SubjectName);
            this.encryptProvidor = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            this.decryptProvidor = (RSACryptoServiceProvider)certificate.PrivateKey;
        }


        /// <summary>
        /// Singleton Accessor
        /// </summary>
        public static TokenCryptoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new TokenCryptoManager();
                        }
                    }
                }

                return instance;
            }
        }

        public string Encrypt(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token");
            }

            return Convert.ToBase64String(
                this.encryptProvidor.Encrypt(Encoding.UTF8.GetBytes(token), false));
        }

        public string Decrypt(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                throw new ArgumentException("encrypted");
            }

            return Encoding.UTF8.GetString(
                this.decryptProvidor.Decrypt(Convert.FromBase64String(encrypted), false));
        }

        public X509Certificate2 Certificate
        {
            get
            {
                return this.certificate;
            }
        }

        private X509Certificate2 GetX509Certificate(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName))
            {
                throw new ArgumentException("subjectName");
            }

            X509Store store = null;
            try
            {
                store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

                // For now just take the first one found
                if (certs.Count > 0)
                {
                    return certs[0];
                }
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
            }

            throw new ArgumentException("certificate not found by subject name");
        }
    }
}
