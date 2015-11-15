
using JJ.SecureTokenService.Authentication;
using JJ.SecureTokenService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JJ.SecureTokenService.Services
{
    public sealed class ServiceLocator
    {
        readonly Dictionary<string, IAuthenticationProvider> authProviders = 
            new Dictionary<string, IAuthenticationProvider>(StringComparer.OrdinalIgnoreCase);

        static volatile ServiceLocator instance;
        static object syncRoot = new Object();

        private ServiceLocator()
        {
            var config = System.Configuration.ConfigurationManager.GetSection("services") as ServiceLocatorSection;
            foreach (var el in config.Instances)
            {
                var element = el as ServiceInstanceElement;
                if (element != null)
                {
                    if (element.ProviderType.Equals("AuthProvider", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var instance = Activator.CreateInstanceFrom(element.Assembly, element.Type);
                            var type = (IAuthenticationProvider)instance.Unwrap();
                            authProviders.Add(element.Name, type);
                        }
                        catch
                        {
                            // log failure
                        }
                    }
                }
            }
        }

        public static ServiceLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ServiceLocator();
                    }
                }

                return instance;
            }
        }

        public IList<IAuthenticationProvider> GetAuthenticationProviders()
        {
            return this.authProviders.Values.ToList();
        }

        public IAuthenticationProvider GetAuthenticationProvider(string name)
        {
            IAuthenticationProvider provider = null;
            if(!this.authProviders.TryGetValue(name, out provider))
            {
                //log failure
            }

            return provider;
        }
    }
}