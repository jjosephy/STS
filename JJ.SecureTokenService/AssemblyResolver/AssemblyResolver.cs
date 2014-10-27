using JJ.SecureTokenService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Dispatcher;

namespace JJ.SecureTokenService
{
    /// <summary>
    /// Hook for testing
    /// </summary>
    public class AssemblyResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new Assembly[1]
            {
                // Add more handlers here
                typeof(TokenController).Assembly
            };
        }
    }
}