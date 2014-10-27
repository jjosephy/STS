
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace OwinSelfHostTestServer
{
    class TestAssemblyResolver : DefaultAssembliesResolver
    {
        static Dictionary<string, Type> externalTypes = new Dictionary<string, Type>();
        static object sync = new object();

        internal static IDictionary<string, Type> ExternalTypes
        {
            get
            {
                return externalTypes;
            }
        }

        internal static void RegisterTypes(Type[] types)
        {
            if (types != null)
            {
                foreach (var type in types)
                {
                    lock (sync)
                    {
                        if (!externalTypes.ContainsKey(type.Name))
                        {
                            externalTypes.Add(type.Name, type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the list of assemblies to load controllers for
        /// </summary>
        /// <returns>A collection of assemblies to load</returns>
        public override ICollection<Assembly> GetAssemblies()
        {
            if (ExternalTypes.Any())
            {
                var types = ExternalTypes.Select(fx => fx.Value)
                    .Select(ax => ax.Assembly).ToArray();
                return types;
            }

            return new Assembly[0];
        }
    }
}
