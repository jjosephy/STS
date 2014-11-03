using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JJ.SecureTokenService.Tests
{
    internal class CertificateManager
    {
        public static X509Certificate GetClientCert(string certName)
        {
            X509Store store = null;
            try
            {
                store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindBySubjectName, certName, true);

                if (certs.Count > 0)
                {
                    return certs[0];
                }
            }
            finally
            {
                if (store != null)
                    store.Close();
            }

            return null;
        }
    }
}
